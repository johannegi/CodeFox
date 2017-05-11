using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CodeFox.Services
{
    public class ProjectService
    {
        private FileService FService = new FileService();
        private FolderService FoService = new FolderService();
        private ApplicationDbContext DB = new ApplicationDbContext();

        public ProjectsViewModel GetProjectsViewModel(string Username)
        {
            if(Username == "")
            {
                throw new Exception();
            }
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
                   // UserView.TimeAgo = string.Format("{ 0:MM / dd / yyy HH: mm: ss.fff}",tmp.DateModified);
                    UserView.Projects.Add(tmp);
                }
            }
    
            var PShare = DB.ProjectShares.Where(x => x.ShareUser.ID == user.ID).ToList();
            if (PShare != null)
            {
                foreach (var item in PShare)
                {
                    Project tmp = DB.Projects.Find(item.ShareProject.ID);
                 //   UserView.TimeAgo = string.Format("{ 0:MM / dd / yyy HH: mm: ss.fff}", tmp.DateModified);
                    UserView.SharedProjects.Add(tmp);
                }
            }

            return UserView;
        }

        public EditorViewModel GetEditorViewModel(int? ProjectID)
        {
            if (!ProjectID.HasValue)
            {
                throw new Exception();
            }
            EditorViewModel projectView = new EditorViewModel();

            Project CurrProject = DB.Projects.Find(ProjectID);
            projectView.Files = new List<File>();
            projectView.SharedWith = new List<UserInfo>();

            projectView.Name = CurrProject.Name;
            projectView.Owner = CurrProject.Owner;
            projectView.ReadMe = CurrProject.ReadMe;
            projectView.Type = CurrProject.Type;
            projectView.ID = (int)ProjectID;
            projectView.CurrentOpenFile = projectView.ReadMe;
            projectView.FileToOpenID = projectView.ReadMe.ID;
            


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

            projectView.Folders = DB.Folders.Where(x => x.ProjectStructure.ID == CurrProject.ID).ToList();

            return projectView;
        }

        public bool CanUserOpenProject(int id, string Username)
        {
            if(Username == "")
            {
                throw new Exception();
            }
            EditorViewModel model = GetEditorViewModel(id);
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

        public bool CreateProject (CreateProjectViewModel NewCreateProject, string Username)
        {
            if(NewCreateProject == null || Username == "")
            {
                return false;
            }
            var ProjectWithSameName = DB.Projects.Where(x => x.Owner.Username == Username
                                                        && x.Name == NewCreateProject.Name).FirstOrDefault();
            if(ProjectWithSameName != null )
            {
                return false;
            }
            UserInfo Owner = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
            Project NewProject = new Project();
            NewProject.Name = NewCreateProject.Name;
            NewProject.Type = NewCreateProject.Type;
            NewProject.Owner = Owner;
            NewProject.ReadMe = FService.CreateReadMe(NewCreateProject.ReadMe);
            NewProject.DateCreated = DateTime.Now;
            NewProject.DateModified = DateTime.Now;

            if(NewProject.Type == "Web Application")
            {
                var Files = FService.CreateWebApplication();
                FileInProject FIProjectCSS = new FileInProject();
                FIProjectCSS.FileProject = NewProject;
                FIProjectCSS.ProjectFile = Files[0];

                FileInProject FIProjectJS = new FileInProject();
                FIProjectJS.FileProject = NewProject;
                FIProjectJS.ProjectFile = Files[1];

                DB.FilesInProjects.Add(FIProjectCSS);
                DB.FilesInProjects.Add(FIProjectJS);
            }

            // þarf að vera DB.SaveChanges tvisvar?
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
            return true;
        }

        public Project GetProjectFromID(int? ProjectID)
        {
            Project ProjectWithID = DB.Projects.Where(x => x.ID == ProjectID).SingleOrDefault();
            if(ProjectWithID == null)
            {
                throw new Exception();
            }
            return ProjectWithID;
        }

        public bool AddCollaborator(string Username, int? ProjectID)
        {
            if(ProjectID.HasValue && Username != "")
            {
                var CollaboratorAlreadyExists = DB.ProjectShares.Where(x => x.ShareUser.Username == Username
                                                       && x.ShareProject.ID == ProjectID).SingleOrDefault();
                var Owner = (from x in DB.Projects where x.ID == ProjectID select x.Owner).SingleOrDefault();
                if (CollaboratorAlreadyExists == null && Username != Owner.Username)
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
            }
            return false;
        }

        public bool RemoveCollaborator(string Username, int? ProjectID)
        {
            if(ProjectID.HasValue && Username != "")
            {
                UserService getUser = new UserService();
                var SharedUsers = getUser.GetSharedUsersFromProject(ProjectID);
                var RemoveUser = SharedUsers.Where(x => x.Username == Username).SingleOrDefault();
                if(RemoveUser != null)
                {
                    ProjectShare DeleteShare = (from x in DB.ProjectShares where x.ShareUser.Username == Username
                                                && x.ShareProject.ID == ProjectID select x).SingleOrDefault();
                    if(DeleteShare != null)
                    {
                        DB.ProjectShares.Remove(DeleteShare);
                        DB.SaveChanges();
                        return true;
                    }
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

        public void DeleteProject(int? ProjectID)
        {
            if(!ProjectID.HasValue)
            {
                throw new Exception();
            }
            //Deleting connection between owner and project to delete
            ProjectOwner POwner = DB.ProjectOwners.Where(x => x.OwnerProject.ID == ProjectID).FirstOrDefault();
            DB.ProjectOwners.Remove(POwner);

            var FileProject = DB.FilesInProjects.Where(x => x.FileProject.ID == ProjectID).ToList();
            //Looping through all connections between the project to delete and files
            foreach(var item in FileProject)
            {
                File TheFile = DB.Files.Where(x => x.ID == item.ProjectFile.ID).FirstOrDefault();
                DB.Files.Remove(TheFile); //Deleting the file
                DB.FilesInProjects.Remove(item); //Deleting the connection
            }
            var Folders = DB.Folders.Where(x => x.ProjectStructure.ID == ProjectID);
            foreach(var item in Folders)
            {
                DB.Folders.Remove(item);
            }
            var Shares = DB.ProjectShares.Where(x => x.ShareProject.ID == ProjectID);
            foreach(var item in Shares)
            {
                DB.ProjectShares.Remove(item);
            }
            Project TheProject = DB.Projects.Where(x => x.ID == ProjectID).FirstOrDefault();
            DB.Files.Remove(TheProject.ReadMe);
            DB.Projects.Remove(TheProject);

            DB.SaveChanges();
            
        }

        //Creates all folder structure in specific path and writes down all project in it
        public void ExportProjectToDirectory(int? ProjectID, string Path)
        {
            if(!ProjectID.HasValue)
            {
                throw new Exception();
            }
            List<FileInProject> FileProject = DB.FilesInProjects.Where(x => x.FileProject.ID == ProjectID).ToList();
            List<File> AllFiles = new List<File>();
            foreach(FileInProject item in FileProject)  //All files in the project are set in a list
            {
                AllFiles.Add(item.ProjectFile);
            }
            Project TheProject = GetProjectFromID(ProjectID); //Get project to add ReadMe file to AllFiles
            File ReadMe = DB.Files.Where(x => x.ID == TheProject.ReadMe.ID).FirstOrDefault();
            AllFiles.Add(ReadMe);

            FoService.CreateTempProjectFolders(ProjectID, Path); //Folder structure for project created

            foreach(File file in AllFiles) //All files written down in right folders
            {
                string text = file.Location;
                string FilePath = Path + FoService.GetFolderPath(file.FolderStructure);
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(FilePath  +  @"\" + file.Name + "." + file.Type))
                {
                    outputFile.WriteLine(text);
                }
            }
        }
        public byte[] GetZippedProject(string GetPath, string SetPath)
        {
            System.IO.Directory.CreateDirectory(SetPath);
            string FilePath = SetPath + "/tempProject.zip";


            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(GetPath);
                zip.Save(FilePath);
                return System.IO.File.ReadAllBytes(FilePath);
            }
        }
        public List<Project> SearchShared(string Term, string Username)
        {
            var User = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
            int UserID = User.ID;
            if(Term != null && User != null)
            {
                // If we don't find anything we return null eitherway, hence if(FoundProjects == null) not neccessary
                var FoundProjectConnections = (from x in DB.ProjectShares where x.ShareUser.ID == UserID 
                                     select x.ShareProject).ToList();
                var FoundProjects = FoundProjectConnections.Where(x => x.Name.StartsWith(Term)).ToList();
                return FoundProjects;
            }
            return null;
        }

        public List<Project> SearchOwned(string Term, string Username)
        {
            var User = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
            int UserID = User.ID;
            if (Term != null && User != null)
            {
                // If we don't find anything we return null eitherway, hence if(FoundProjects == null) not neccessary
                var FoundProjects = (from x in DB.Projects
                                     where x.Owner.ID == UserID
                                     && x.Name.StartsWith(Term)
                                     select x).ToList();
                return FoundProjects;
            }
            return null;
        }

        public List<Project> Sorted(string Method, bool Ascending)
        {
            const string DM = "Date Modified";
            const string DC = "Date Created";
            const string N = "Name";
            const string O = "Owner";
            if (Method != null && Method != "")
            {
                // If we don't find anything we return null eitherway, hence if(FoundProjects == null) not neccessary
                var SortingMethod = "";
                if(Method == N)
                {
                    SortingMethod = N;
                }
                else if(Method == DC)
                {
                    SortingMethod = DC;
                }
                else if(Method == DM)
                {
                    SortingMethod = DM;
                }
                else if(Method == O)
                {
                    SortingMethod = O;
                }
                else
                {
                    return null;
                }
                List<Project> SortedProjects = new List<Project>();
                if(Ascending)
                {
                    SortedProjects = (from x in DB.Projects orderby SortingMethod select x).ToList();
                }                
                else
                {
                    SortedProjects = (from x in DB.Projects orderby SortingMethod descending select x).ToList();
                }
                return SortedProjects;
            }
            return null;
        }
    }
}