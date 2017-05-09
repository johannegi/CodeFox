using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Models.Entities
{
    public class File
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        [AllowHtml]
        public string Location { get; set; }
        public Folder FolderStructure { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}