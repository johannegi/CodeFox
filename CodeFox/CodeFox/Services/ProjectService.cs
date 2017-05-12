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
        private readonly FileService FService;
        private readonly FolderService FoService;
        private readonly IAppDataContext DB;

        public ProjectService(IAppDataContext context)
        {
            DB = context ?? new ApplicationDbContext();
            FService = new FileService(context);
            FoService = new FolderService(context);
        }

        //Returns model which is used to get information to use for index of user's project page
        public ProjectsViewModel GetProjectsViewModel(string Username)
        {
            if(Username == "")
            {
                throw new Exception();
            }
            //Gets UserInfo of user with specific Username
            UserInfo user = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();

            ProjectsViewModel UserView = new ProjectsViewModel(); //Creates Viewmodel which is later returned
            UserView.Projects = new List<Project>();
            UserView.SharedProjects = new List<Project>();
            UserView.ID = user.ID;
            UserView.Name = user.Name;
            UserView.Username = user.Username;
            UserView.Country = user.Country;
            UserView.Email = user.Email;
      
            //Gets all project that specific user owns and sets them to a list
            var POwners = DB.ProjectOwners.Where(x => x.Owner.ID == user.ID).ToList();
            if (POwners != null)
            {
                foreach (var item in POwners) //All projects that specific user owns are set to the viewmodel Projects list
                {
                    Project tmp = GetProjectFromID(item.OwnerProject.ID);
                    UserView.Projects.Add(tmp);
                }
            }
    
            //All Projects shared with specific user are set in a list
            var PShare = DB.ProjectShares.Where(x => x.ShareUser.ID == user.ID).ToList();
            if (PShare != null)
            {
                foreach (var item in PShare) //Loops through all projects shared with specific user and set to the viewmodel SharedProjects list
                {
                    Project tmp = DB.Projects.Find(item.ShareProject.ID);
                    UserView.SharedProjects.Add(tmp);
                }
            }
            //Viewmodel is returned
            return UserView;
        }

        //Returns Viewmodel for the editor page for a project with specific ID
        public EditorViewModel GetEditorViewModel(int? ProjectID)
        {
            if (!ProjectID.HasValue)
            {
                throw new Exception();
            }
            //Viewmodel which is later returned is created
            EditorViewModel projectView = new EditorViewModel();

            Project CurrProject = GetProjectFromID(ProjectID); //Project to open
            projectView.Files = new List<File>();
            projectView.SharedWith = new List<UserInfo>();

            //Values are set of viewmodel
            projectView.Name = CurrProject.Name;
            projectView.Owner = CurrProject.Owner;
            projectView.ReadMe = CurrProject.ReadMe;
            projectView.Type = CurrProject.Type;
            projectView.ID = (int)ProjectID;
            projectView.CurrentOpenFile = projectView.ReadMe;
            projectView.FileToOpenID = projectView.ReadMe.ID;
            

            //Gets all connections between file and project to open as a list
            var FilesProject = DB.FilesInProjects.Where(x => x.FileProject.ID == CurrProject.ID).ToList();
            if (FilesProject != null)
            {
                foreach (var item in FilesProject) //Adds all files from project to open to the viewmodel
                {
                    File tmp = item.ProjectFile;
                    projectView.Files.Add(tmp);
                }
            }

            //Gets all connections between users and project to open as a list
            var Shared = DB.ProjectShares.Where(x => x.ShareProject.ID == CurrProject.ID).ToList();
            if (Shared != null)
            {
                foreach (var item in Shared) //All users that have project shared with them are added to the viewmodel
                {
                    UserInfo tmp = item.ShareUser;
                    projectView.SharedWith.Add(tmp);
                }
            }

            //All folders in project to open are added to the viewmodel as a list
            projectView.Folders = DB.Folders.Where(x => x.ProjectStructure.ID == CurrProject.ID).ToList();

            //Viewmodel is returned
            return projectView;
        }

        //Returns true if user with specific username has access to project with specific ID, otherwise false
        public bool CanUserOpenProject(int id, string Username)
        {
            if(Username == "")
            {
                throw new Exception();
            }
            EditorViewModel model = GetEditorViewModel(id); //Gets viewmodel of project
            if (model.Owner.Username == Username) //If the user is it's owner the function returns true
            {
                return true;
            }
            else
            {
                foreach (var item in model.SharedWith) //If the user has the project shared with him the function returns true
                {
                    if (item.Username == Username)
                    {
                        return true;
                    }
                }
            }
            return false; //Otherwise the function returns false
        }

        //Creates a project with User withc specific username as owner. Returns false if fails, otherwise true
        public bool CreateProject (CreateProjectViewModel NewCreateProject, string Username)
        {
            if(NewCreateProject == null || Username == "")
            {
                return false;
            }
            var ProjectWithSameName = DB.Projects.Where(x => x.Owner.Username == Username
                                                        && x.Name == NewCreateProject.Name).FirstOrDefault();
            if(ProjectWithSameName != null ) //If user has created a project with same name nothing is created and function returns false
            {
                return false;
            }

            UserInfo Owner = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault(); //User that is creating project
            Project NewProject = new Project(); //The project that is being made, gets values from viewmodel
            NewProject.Name = NewCreateProject.Name;
            NewProject.Type = NewCreateProject.Type;
            NewProject.Owner = Owner;
            NewProject.ReadMe = FService.CreateReadMe(NewCreateProject.ReadMe); //New ReadMe created
            NewProject.DateCreated = DateTime.Now;
            NewProject.DateModified = DateTime.Now;

            //If project is a web application more default files are made
            if(NewProject.Type == "Web Application") 
            {
                var Files = FService.CreateWebApplication(); //Gets a list of extra files
                FileInProject FIProjectCSS = new FileInProject(); //connection between file and project is made
                FIProjectCSS.FileProject = NewProject;
                FIProjectCSS.ProjectFile = Files[0]; //css file
                
                FileInProject FIProjectJS = new FileInProject(); //connection between file and project is made
                FIProjectJS.FileProject = NewProject;
                FIProjectJS.ProjectFile = Files[1]; //js file

                DB.FilesInProjects.Add(FIProjectCSS); //added to database
                DB.FilesInProjects.Add(FIProjectJS);
            }
            
            DB.Projects.Add(NewProject); //Project added to database
            DB.SaveChanges();
            
            //Connection between user creating and project is made
            ProjectOwner POwner = new ProjectOwner();
            POwner.Owner = Owner;
            POwner.OwnerProject = NewProject;
            //Connection between default file and project is made
            FileInProject FIProject = new FileInProject();
            FIProject.FileProject = NewProject;
            FIProject.ProjectFile = FService.CreateDefaultFile(NewProject.Type); //default file is made

            DB.ProjectOwners.Add(POwner); //connections added to database
            DB.FilesInProjects.Add(FIProject);
            DB.SaveChanges(); //changes saved and function returns true
            return true;
        }

        //Gets project with specific ID and returns it
        public Project GetProjectFromID(int? ProjectID)
        {
            Project ProjectWithID = DB.Projects.Where(x => x.ID == ProjectID).SingleOrDefault();
            if(ProjectWithID == null)
            {
                throw new Exception();
            }
            return ProjectWithID;
        }

        //Shares Project with specific ID with user with specific Username. Returns true if connection is made, otherwise false
        public bool AddCollaborator(string Username, int? ProjectID)
        {
            if(ProjectID.HasValue && Username != "")
            {
                var CollaboratorAlreadyExists = DB.ProjectShares.Where(x => x.ShareUser.Username == Username //If the User getting project shared with has project
                                                       && x.ShareProject.ID == ProjectID).SingleOrDefault(); //already shared with him or is owner it is saved in either of those variables
                var Owner = (from x in DB.Projects where x.ID == ProjectID select x.Owner).SingleOrDefault();
                if (CollaboratorAlreadyExists == null && Username != Owner.Username) //if user with specific username did not have access to project the new connection is made
                {
                    ProjectShare NewConnection = new ProjectShare();
                    NewConnection.ShareUser = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
                    NewConnection.ShareProject = DB.Projects.Where(x => x.ID == ProjectID).SingleOrDefault();
                    
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
                UserService getUser = new UserService(null);
                var SharedUsers = getUser.GetSharedUsersFromProject(ProjectID);
                var RemoveUser = SharedUsers.Where(x => x.Username == Username).FirstOrDefault();
                if(RemoveUser != null)
                {
                    ProjectShare DeleteShare = (from x in DB.ProjectShares where x.ShareUser.Username == Username
                                                && x.ShareProject.ID == ProjectID select x).FirstOrDefault();
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
            //Looping through all folders in project and deletes them
            foreach(var item in Folders)
            {
                DB.Folders.Remove(item);
            }

            var Shares = DB.ProjectShares.Where(x => x.ShareProject.ID == ProjectID);
            //Looping through all connection between project and shared users and deletes them
            foreach(var item in Shares)
            {
                DB.ProjectShares.Remove(item);
            }
            Project TheProject = DB.Projects.Where(x => x.ID == ProjectID).FirstOrDefault();
            DB.Files.Remove(TheProject.ReadMe); //Deletes readme file of project
            DB.Projects.Remove(TheProject);     //Finally deletes project itself

            DB.SaveChanges(); //saves changes to database
            
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

            FoService.CreateTempProjectFolders(ProjectID, Path); //Folder structure for project created on drive

            foreach(File file in AllFiles) //All files written down in their correct folder that was created on drive
            {
                string text = file.Location;
                string FilePath = Path + FoService.GetFolderPath(file.FolderStructure);
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(FilePath  +  @"\" + file.Name + "." + file.Type))
                {
                    outputFile.WriteLine(text);
                }
            }
        }

        //Makes a .zip file of everything in GetPath and saves it in SetPath
        //and returns the .zip file as an array of bytes
        public byte[] GetZippedProject(string GetPath, string SetPath)
        {
            System.IO.Directory.CreateDirectory(SetPath); //Directory is created to put .zip file in
            string FilePath = SetPath + "/tempProject.zip"; //Path of .zip file


            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(GetPath); //zippes everything in GetPath
                zip.Save(FilePath);     //saves the zip file in FilePath
                return System.IO.File.ReadAllBytes(FilePath); //Returns the zip file as an array of bytes
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