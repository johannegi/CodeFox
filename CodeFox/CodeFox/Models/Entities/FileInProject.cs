using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class FileInProject
    {
        public int ID { get; set; }
        public virtual File ProjectFile { get; set; }
        public virtual Project FileProject { get; set; }
    }
}