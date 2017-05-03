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

        public EditorViewModel GetEditorViewModel(int? ProjectID)
        {
            EditorViewModel projectView = new EditorViewModel();

            Project CurrProject = db.Projects.Find(ProjectID);
            projectView.Files = new List<File>();
            projectView.SharedWith = new List<UserInfo>();

            projectView.Name = CurrProject.Name;
            projectView.Owner = CurrProject.Owner;
            projectView.ReadMe = CurrProject.ReadMe;
            projectView.Type = CurrProject.Type;

            var FilesProject = db.FilesInProjects.Where(x => x.FileProject.ID == CurrProject.ID).ToList();
            if (FilesProject != null)
            {
                foreach (var item in FilesProject)
                {
                    File tmp = item.ProjectFile;
                    projectView.Files.Add(tmp);
                }
            }

            var Shared = db.ProjectShares.Where(x => x.ShareProject.ID == CurrProject.ID).ToList();
            if (Shared != null)
            {
                foreach (var item in Shared)
                {
                    UserInfo tmp = item.ShareUser;
                    projectView.SharedWith.Add(tmp);
                }
            }
            

            return projectView;
        }

        public bool CanUserOpenProject(EditorViewModel model, string Username)
        {
            if (model.Owner.Username == Username)
            {
                return true;
            }
            else
            {
                foreach (var item in model.SharedWith)
                {
                    if (item.Username == Username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}