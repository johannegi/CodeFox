using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public virtual UserInfo Owner { get; set; }
        public virtual File ReadMe { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}