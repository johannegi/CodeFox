using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class Folder
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual Project ProjectStructure { get; set; }
        public virtual Folder FolderStructure { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}