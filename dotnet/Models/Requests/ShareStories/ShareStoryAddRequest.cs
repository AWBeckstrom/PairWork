using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests.ShareStories
{
    public class ShareStoryAddRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Email { get; set; }

        [Required]
        [StringLength(3000, MinimumLength = 2)]
        public string Story { get; set; }
        public List<int> FileIds { get; set; }
    }
}
