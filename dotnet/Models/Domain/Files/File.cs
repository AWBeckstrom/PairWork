using Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain.Files
{
    public class File : BaseFile
    {
        public string Name { get; set; }
        public LookUp FileType { get; set; } 
        public bool IsDeleted { get; set; }
        public BaseUser CreatedBy { get; set; } 
        public DateTime DateCreated { get; set; }
      //  public Paged<File> PagedFile { get; set; }
    }
}
