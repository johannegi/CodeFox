using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.ViewModels
{
    public class ProjectsViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public List<Project> Projects { get; set; }
        public List<Project> SharedProjects { get; set; }
        public string TimeAgo { get; set;  }
    }
}