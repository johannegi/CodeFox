using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class ProjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ProjectsViewModel GetProjectsViewModel(string Username)
        {
            UserInfo user = db.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();

            ProjectsViewModel UserView = new ProjectsViewModel();
            UserView.Projects = new List<Project>();
            UserView.SharedProjects = new List<Project>();
            UserView.ID = user.ID;
            UserView.Name = user.Name;
            UserView.Username = user.Username;
            UserView.Country = user.Country;
            UserView.Email = user.Email;

            var POwners = db.ProjectOwners.Where(x => x.Owner.ID == user.ID).ToList();
            if (POwners != null)
            {
                foreach (var item in POwners)
                {
                    
                    Project tmp = db.Projects.Find(item.OwnerProject.ID);
                    UserView.Projects.Add(tmp);
                }
            }
    
            var PShare = db.ProjectShares.Where(x => x.ShareUser.ID == user.ID).ToList();
            if (PShare != null)
            {
                foreach (var item in PShare)
                {
                    Project tmp = db.Projects.Find(item.ShareProject.ID);
                    UserView.SharedProjects.Add(tmp);
                }
            }

            return UserView;
        }

        public EditorViewModel GetEditorViewModel(int ProjectID)
        {
            EditorViewModel projectView = new EditorViewModel();

            Project CurrProject = db.Projects.Find(ProjectID);

            projectView.Name = CurrProject.Name;
            projectView.Owner = CurrProject.Owner;
            projectView.ReadMe = CurrProject.ReadMe;
            projectView.Type = CurrProject.Type;



            return projectView;
        }

    }
}