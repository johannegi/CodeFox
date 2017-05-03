using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class ProjectOwner
    {
        public int ID { get; set; }
        public virtual UserInfo Owner { get; set; }
        public virtual Project OwnerProject { get; set; }
    }
}