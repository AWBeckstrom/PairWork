using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Models;
using Models.Domain.Files;
using Models.Requests.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Amazon.Runtime;
using Models.AppSettings;
using Models.Enums;
using System.Data.SqlClient;

namespace Services.Interfaces
{
    public interface IFileService
    {
        List<File> GetAll();

        int Add(FileAddRequest model, int userId);

        Paged<File> Pagination(int pageIndex, int pageSize, int userId, bool showDeleted);

        void Delete(int Id, int status);

        Task<List<BaseFile>> UploadFileAsync(List<IFormFile> files, int userId);
        Paged<File> SearchPaginated(int pageIndex, int pageSize, string search, bool showDeleted);
        void DeleteFile(int Id);
    }
}