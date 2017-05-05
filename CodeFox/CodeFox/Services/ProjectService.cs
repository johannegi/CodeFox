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
        private FileService FService = new FileService();
        private ApplicationDbContext DB = new ApplicationDbContext();

        public ProjectsViewModel GetProjectsViewModel(string Username)
        {
            UserInfo user = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();

            ProjectsViewModel UserView = new ProjectsViewModel();
            UserView.Projects = new List<Project>();
            UserView.SharedProjects = new List<Project>();
            UserView.ID = user.ID;
            UserView.Name = user.Name;
            UserView.Username = user.Username;
            UserView.Country = user.Country;
            UserView.Email = user.Email;

            var POwners = DB.ProjectOwners.Where(x => x.Owner.ID == user.ID).ToList();
            if (POwners != null)
            {
                foreach (var item in POwners)
                {
                    
                    Project tmp = DB.Projects.Find(item.OwnerProject.ID);
                    UserView.Projects.Add(tmp);
                }
            }
    
            var PShare = DB.ProjectShares.Where(x => x.ShareUser.ID == user.ID).ToList();
            if (PShare != null)
            {
                foreach (var item in PShare)
                {
                    Project tmp = DB.Projects.Find(item.ShareProject.ID);
                    UserView.SharedProjects.Add(tmp);
                }
            }

            return UserView;
        }

        public EditorViewModel GetEditorViewModel(int? ProjectID)
        {
            EditorViewModel projectView = new EditorViewModel();

            Project CurrProject = DB.Projects.Find(ProjectID);
            projectView.Files = new List<File>();
            projectView.SharedWith = new List<UserInfo>();

            projectView.Name = CurrProject.Name;
            projectView.Owner = CurrProject.Owner;
            projectView.ReadMe = CurrProject.ReadMe;
            projectView.Type = CurrProject.Type;
            projectView.ID = (int)ProjectID;
            projectView.CurrentOpenFile = CurrProject.ReadMe;


            var FilesProject = DB.FilesInProjects.Where(x => x.FileProject.ID == CurrProject.ID).ToList();
            if (FilesProject != null)
            {
                foreach (var item in FilesProject)
                {
                    File tmp = item.ProjectFile;
                    projectView.Files.Add(tmp);
                }
            }

            var Shared = DB.ProjectShares.Where(x => x.ShareProject.ID == CurrProject.ID).ToList();
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

        public void CreateProject (CreateProjectViewModel NewCreateProject, string Username)
        {
            UserInfo Owner = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
            Project NewProject = new Project();
            NewProject.Name = NewCreateProject.Name;
            NewProject.Type = NewCreateProject.Type;
            NewProject.Owner = Owner;
            NewProject.ReadMe = FService.CreateReadMe(NewCreateProject.ReadMe);
            NewProject.DateCreated = DateTime.Now;
            NewProject.DateModified = DateTime.Now;

            DB.Projects.Add(NewProject);
            DB.SaveChanges();
            
            ProjectOwner POwner = new ProjectOwner();
            POwner.Owner = Owner;
            POwner.OwnerProject = NewProject;

            FileInProject FIProject = new FileInProject();
            FIProject.FileProject = NewProject;
            FIProject.ProjectFile = FService.CreateDefaultFile(NewProject.Type);

            DB.ProjectOwners.Add(POwner);
            DB.FilesInProjects.Add(FIProject);
            DB.SaveChanges();
        }

        public Project GetProjectFromID(int? ProjectID)
        {
            Project ProjectWithID = DB.Projects.Where(x => x.ID == ProjectID).SingleOrDefault();
            if(ProjectWithID == null)
            {
                //TODO:implement error
            }
            return ProjectWithID;
        }

        public bool AddCollaborator(string Username, int? ProjectID)
        {
            var CollaboratorAlreadyExists = DB.ProjectShares.Where(x => x.ShareUser.Username == Username 
                                                                   && x.ShareProject.ID == ProjectID).SingleOrDefault();
            if (CollaboratorAlreadyExists == null)
            {
                ProjectShare NewConnection = new ProjectShare();
                NewConnection.ShareUser = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
                NewConnection.ShareProject = DB.Projects.Where(x => x.ID == ProjectID).SingleOrDefault();
                NewConnection.ID = (from n in DB.ProjectShares orderby n.ID descending select n.ID).FirstOrDefault();
                NewConnection.ID++;
                if (NewConnection.ShareUser != null)
                {
                    DB.ProjectShares.Add(NewConnection);
                    DB.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public List<string> GetTypeList()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Lists/ProjectTypes.txt");
            List<string> listinn = new List<string>(System.IO.File.ReadLines(path).ToList());
            return listinn;
        }
    }
}