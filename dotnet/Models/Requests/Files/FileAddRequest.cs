using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests.Files
{
    public class FileAddRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string Url { get; set; }

        [Required]
        [Range(1, 25)]
        public int FileTypeId { get; set; }
    }
}
