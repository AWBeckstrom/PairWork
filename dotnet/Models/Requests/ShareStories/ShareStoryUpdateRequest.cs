using System.ComponentModel.DataAnnotations;

namespace Models.Requests.ShareStories
{
    public class ShareStoryUpdateRequest : ShareStoryAddRequest, IModelIdentifier
    {
        public int Id { get; set; }
    }
}
