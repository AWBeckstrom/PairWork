using Models.Domain.Users;
using System;

namespace Models.Domain.ShareStories
{
    public class ShareStory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Story { get; set; }
        public BaseUser CreatedBy { get; set; }
        public bool IsApproved { get; set; }
        public BaseUser ApprovedBy { get; set; }
        public DateTime DateCreated { get; set; }

        public string StoryFileUrl { get; set; }

        public bool IsDeleted { get; set; }
    }

}
