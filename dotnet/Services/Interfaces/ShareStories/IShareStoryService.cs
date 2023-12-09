using Models;
using Models.Domain.ShareStories;
using Models.Requests.ShareStories;

namespace Sabio.Services.Interfaces.ShareStories
{
    public interface IShareStoryService
    {
        int Insert(ShareStoryAddRequest model, int userId);
        void Update(ShareStoryUpdateRequest model, int userId);
        void UpdateApproval(int id, int userId);
        void UpdateIsDeleted(int id );

        Paged<ShareStory> SelectByApproval(int pageIndex, int pageSize);
        Paged<ShareStory> SelectByNonApproval(int pageIndex, int pageSize);

        Paged<ShareStory> SelectByIsDeleted(int pageIndex, int pageSize);
        ShareStory Get(int id);
        Paged<ShareStory> SelectAll(int pageIndex, int pageSize);
        void Delete(int id);

    }

}