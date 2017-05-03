using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.ViewModels
{
    public class EditorViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public UserInfo Owner { get; set; }
        public File ReadMe { get; set; }
        public List<File> Files { get; set; }
        public List<UserInfo> SharedWith { get; set; }
    }
}