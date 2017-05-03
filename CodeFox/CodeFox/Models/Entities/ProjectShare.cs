using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class ProjectShare
    {
        public int ID { get; set; }
        public virtual UserInfo ShareUser { get; set; }
        public virtual Project ShareProject { get; set; }
    }
}