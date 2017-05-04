using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.ViewModels
{
    public class ShareProjectViewModel
    {
        public Project ShareProject { get; set; }
        public List<UserInfo> AllUsers { get; set; }
        public List<UserInfo> SharedWith { get; set; }
    }
}