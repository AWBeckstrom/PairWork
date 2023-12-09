using Data.Providers;
using System.Data.SqlClient;
using System.Data;
using Data;
using System.Collections.Generic;
using Models;
using Models.Requests.ShareStories;
using Services.Interfaces.ShareStories;
using Models.Domain.ShareStories;
using Models.Domain.Users;

namespace Services.ShareStories
{
    public class ShareStoryService : IShareStoryService
    {
        private IDataProvider _data;
        public ShareStoryService(IDataProvider data)
        {
            _data = data;
        }

        public int Insert(ShareStoryAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[ShareStory_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@CreatedBy", userId);
                    DataTable fileIdTable = new DataTable();
                    fileIdTable.Columns.Add("FileId", typeof(int));

                    if (model.FileIds != null)
                    {
                        foreach (var fileIds in model.FileIds)
                        {
                            fileIdTable.Rows.Add(fileIds);
                        }
                    }

                    SqlParameter tvpParam = col.AddWithValue("@FileId", fileIdTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.FileIdTableType";

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);

                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });

            return id;
        }
        public void Update(ShareStoryUpdateRequest model, int userId)
        {
            string procName = "[dbo].[ShareStory_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@Id", model.Id);
                },
                returnParameters: null);
        }

        public void UpdateApproval(int id, int userId)
        {
            string procName = "[dbo].[ShareStory_UpdateApproval]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@ApprovedBy", userId);
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null);
        }

        public void UpdateIsDeleted(int id )
        {
            string procName = "[dbo].[ShareStory_UpdateIsDeleted]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null);
        }


        public Paged<ShareStory> SelectByApproval(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[ShareStory_SelectByApproval]";
            Paged<ShareStory> pagedList = null;
            List<ShareStory> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                ShareStory shareStory = MapSingleShareStory(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<ShareStory>();
                }
                list.Add(shareStory);
            });
            pagedList = new Paged<ShareStory>(list, pageIndex, pageSize, totalCount);
            return pagedList;
        }


        public Paged<ShareStory> SelectByNonApproval(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[ShareStory_SelectNonApproved]";
            Paged<ShareStory> pagedList = null;
            List<ShareStory> list = null;
            int totalCount = 0; 

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                ShareStory shareStory = MapSingleShareStory(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<ShareStory>();
                }
                list.Add(shareStory);
            });
            pagedList = new Paged<ShareStory>(list, pageIndex, pageSize, totalCount);
            return pagedList;
        }


             public Paged<ShareStory> SelectByIsDeleted(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[ShareStory_SelectByIsDeleted]";
            Paged<ShareStory> pagedList = null;
            List<ShareStory> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                ShareStory shareStory = MapSingleShareStory(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<ShareStory>();
                }
                list.Add(shareStory);
            });
            pagedList = new Paged<ShareStory>(list, pageIndex, pageSize, totalCount);
            return pagedList;
        }




        public ShareStory Get(int Id)
        {
            string procName = "[dbo].[ShareStory_Select_ById]";

            ShareStory shareStory = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", Id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                shareStory = MapSingleShareStory(reader, ref startingIndex);
            }
            );
            return shareStory;
        }

        public Paged<ShareStory> SelectAll(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[ShareStory_SelectAll]";
            Paged<ShareStory> pagedList = null;
            List<ShareStory> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                ShareStory shareStory = MapSingleShareStory(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<ShareStory>();
                }
                list.Add(shareStory);
            });
            pagedList = new Paged<ShareStory>(list, pageIndex, pageSize, totalCount);
            return pagedList;
        }

        public void Delete(int Id)
        {
            string procName = "[dbo].[ShareStory_Delete_ById]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", Id);
            }, returnParameters: null);
        }

        #region Refactors: MapShareStory and addCommonParams
        private static void AddCommonParams(ShareStoryAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@Story", model.Story);



        }

        private static ShareStory MapSingleShareStory(IDataReader reader, ref int startingIndex)
        {
            ShareStory aShareStory = new ShareStory();
            aShareStory.Id = reader.GetSafeInt32(startingIndex++);
            aShareStory.Name = reader.GetSafeString(startingIndex++);
            aShareStory.Email = reader.GetSafeString(startingIndex++);
            aShareStory.Story = reader.GetSafeString(startingIndex++);
            if (!reader.IsDBNull(startingIndex))
            {
                aShareStory.CreatedBy = reader.DeserializeObject<BaseUser>(startingIndex++);
            }
            else
            {
                startingIndex++;
            }
            aShareStory.IsApproved = reader.GetSafeBool(startingIndex++);
            if (!reader.IsDBNull(startingIndex))
            {
                aShareStory.ApprovedBy = reader.DeserializeObject<BaseUser>(startingIndex++);
            }
            else
            {
                startingIndex++;
            }
            aShareStory.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aShareStory.StoryFileUrl = reader.GetSafeString(startingIndex++);
            aShareStory.IsDeleted = reader.GetSafeBool(startingIndex++);
            return aShareStory;
        }
    }
}
#endregion
