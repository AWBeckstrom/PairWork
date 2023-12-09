
using Data;
using Data.Providers;
using Models;
using Models.Domain.Files;
using Models.Requests.Files;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Models.AppSettings;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Models.Enums;
using Models.Domain.Users;
using System.Collections;

namespace Services
{
    public class FileService : IFileService
    {
        IDataProvider _data = null; 
        ILookUpService _lookUpService = null;

        private AWSStorageConfig _awsStorageConfig;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;

        public FileService(IDataProvider data, IOptions<AWSStorageConfig> awsConfig, ILookUpService lookUpService) 
        {
            _data = data;
            _awsStorageConfig = awsConfig.Value;
            _lookUpService = lookUpService;
        }
        public Paged<File> Pagination (int pageIndex, int pageSize, int userId, bool showDeleted)
        {
            Paged<File> pagedList = null;
            List<File> list = null;

            int totalCount = 0;
            string procName = "[dbo].[Files_Select_ByCreatedBy]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection col) 
                {
                    col.AddWithValue("@UserId", userId);
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                    col.AddWithValue("@ShowDeleted", showDeleted);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    File file = MapSingleFile(reader, ref startingIndex);

                    if (totalCount == 0) 
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<File>();
                    }
                    list.Add(file);
                }
                );

            if (list != null) 
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public List<File> GetAll()
        {
            string procName = "[dbo].[Files_SelectAll]";

            List<File> myFile = null;

            _data.ExecuteCmd(procName, inputParamMapper: null,
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    File file = MapSingleFile(reader, ref startingIndex);

                    if (myFile == null)
                    {
                        myFile = new List<File>();
                    }
                    myFile.Add(file);
                });

            return myFile;
        }

        public int Add(FileAddRequest model, int userId) 
        {
            int id = 0;
            string procName = "[dbo].[Files_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col, userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCol)
                {
                    object oId = returnCol["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }
       
        public async Task<List<BaseFile>> UploadFileAsync(List<IFormFile> files, int userId)
        {
            List<BaseFile> list = null;
            var basicAwsCred = new BasicAWSCredentials(_awsStorageConfig.AccessKey, _awsStorageConfig.Secret);

            using (var s3Client = new AmazonS3Client(basicAwsCred, bucketRegion))
            {
                foreach (var file in files)
                {
                    var fileTransferUtility = new TransferUtility(s3Client);
                    string keyName = $"{Guid.NewGuid().ToString()}-{file.FileName}";

                    await fileTransferUtility.UploadAsync(file.OpenReadStream(), _awsStorageConfig.BucketName, $"{keyName}");

                    Console.WriteLine("Upload completed");
                    string url = $"{_awsStorageConfig.Domain}{keyName}";

                    string contentType = file.ContentType;
                    FileType fileType = (FileType)CompareFileTypes(contentType);
                    int fileTypeId = (int)fileType;

                    FileAddRequest fileAddRequest = new FileAddRequest
                    {
                        Name = file.FileName,
                        Url = url,
                        FileTypeId = fileTypeId,
                    };

                    int fileId = Add(fileAddRequest, userId);

                    BaseFile baseFile = new BaseFile
                    {
                        Id = fileId,
                        Url = url
                    };
                    if (list == null)
                    {
                        list = new List<BaseFile>();
                    }
                    list.Add(baseFile);
                }
                return list;
            }
        }
        private int CompareFileTypes(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                    return (int)FileType.ImageJpeg;
                case "application/pdf":
                    return (int)FileType.Pdf;
                case "application/txt":
                    return (int)FileType.Text;
                case "application/docx":
                    return (int)FileType.Docx;
                case "image/png":
                    return (int)FileType.Png;
                case "video/mp4":
                    return (int)FileType.Mp4;
                default:
                    return 0;
            }
        }

       
        public void Delete(int Id, int status)
        {
            string procName = "dbo.FileManager_Delete";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", Id);
                    col.AddWithValue("@status", status);

                },
                returnParameters: null);
        }
        private static void AddCommonParams(FileAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Url", model.Url);
            col.AddWithValue("@FileTypeId", model.FileTypeId);
            col.AddWithValue("@CreatedBy", userId);
        }
       
        private File MapSingleFile(IDataReader reader, ref int startingIndex) 
        {
            File aFile = new File();

            aFile.Id = reader.GetSafeInt32(startingIndex++);
            aFile.Name = reader.GetSafeString(startingIndex++);
            aFile.Url = reader.GetSafeString(startingIndex++);

            aFile.FileType = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            aFile.IsDeleted = reader.GetSafeBool(startingIndex++);

            aFile.CreatedBy = reader.DeserializeObject<BaseUser>(startingIndex++);

            aFile.DateCreated = reader.GetSafeDateTime(startingIndex++);

            return aFile;
        }

        public Paged<File> SearchPaginated(int pageIndex, int pageSize, string search, bool showDeleted)
        {
            Paged<File> pagedList = null;
            List<File> list = new List<File>();
            int totalCount = 0;

            _data.ExecuteCmd("dbo.FileManager_Search_Pagination", inputParamMapper: delegate (SqlParameterCollection col)
            {

                col.AddWithValue("@PageIndex", pageIndex);
                col.AddWithValue("@PageSize", pageSize);
                col.AddWithValue("@Query", search);
                col.AddWithValue("@ShowDeleted",showDeleted);

            }, (IDataReader reader, short set) =>
            {
                int startingIndex = 0;

                File file = MapSingleFile(reader, ref startingIndex);
                totalCount = reader.GetSafeInt32(startingIndex);

                list.Add(file);
            });

            pagedList = new Paged<File>(list, pageIndex, pageSize, list.Count);
            //var pagedata = list.Skip(pageIndex == 0 ? 0 : pageIndex * pageSize).Take(pageSize);
            //if im on the first page I want to skip nothing,
            //from that page forward I always want to skip pageIndex * pageSize to get the next batch of items

            return pagedList;
        }

        public void DeleteFile(int Id)
        {
            string procName = "[dbo].[FileManager_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", Id);
            },
                returnParameters: null);
        }



    }
}
