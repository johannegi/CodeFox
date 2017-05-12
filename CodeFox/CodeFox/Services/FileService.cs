using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class FileService
    {
        private readonly IAppDataContext DB;

        public FileService(IAppDataContext context)
        {
            DB = context ?? new ApplicationDbContext();
        }

        public File CreateReadMe(string Text)
        {
            File ReadMe = new File();
            ReadMe.Name = "ReadMe";
            ReadMe.Type = "txt";
            ReadMe.Location = Text;
            ReadMe.FolderStructure = null;
            ReadMe.DateCreated = DateTime.Now;
            ReadMe.DateModified = DateTime.Now;

            return ReadMe;
        }

        public File CreateDefaultFile(string Type)
        {
            File Default = new File();
            Default.Name = "Index";
            if(Type == "Web Application")
            {
                Default.Type = "html";
                // Default.Location = "<!--This is the default file for this project-->"??;
            }
            else if(Type == "C++")
            {
                Default.Type = "cpp";
            }
            else if(Type == "C#")
            {
                Default.Type = "cs";
            }
            else if(Type == "JavaScript")
            {
                Default.Type = "js";
            }
            else if(Type == "Java")
            {
                Default.Type = "java";
            }
            else
            {
                Default.Type = "txt";
            }
            
            Default.Location = "//This is the default file for this project";
            Default.FolderStructure = null;
            Default.DateCreated = DateTime.Now;
            Default.DateModified = DateTime.Now;

            return Default;
        }

        public List<File> CreateWebApplication()
        {
            File CssFile = new File();
            CssFile.Name = "styles";
            CssFile.Type = "css";
            CssFile.Location = "//This is the default CSS file";
            CssFile.FolderStructure = null;
            CssFile.DateCreated = DateTime.Now;
            CssFile.DateModified = DateTime.Now;

            File JsFile = new File();
            JsFile.Name = "scripts";
            JsFile.Type = "js";
            JsFile.Location = "//This is the default JavaScript file";
            JsFile.FolderStructure = null;
            JsFile.DateCreated = DateTime.Now;
            JsFile.DateModified = DateTime.Now;

            List<File> Files = new List<File>();
            Files.Add(CssFile);
            Files.Add(JsFile);

            return Files;
        } 

        //Checka a USERNAME
        public bool AddFile(AddFilesViewModel Model)
        {
            // UserInfo Owner = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();
            var FileWithSameName = DB.FilesInProjects.Where(x => x.ProjectFile.Name == Model.Name && 
                                                  x.FileProject.ID == Model.ProjectID).FirstOrDefault();
            if(FileWithSameName != null)
            {
                return false;
            }
            //Make the file
            File NewFile = new File();
            NewFile.Name = Model.Name;
            NewFile.Type = Model.Type;
            NewFile.Location = "//This is a new file";
            NewFile.FolderStructure = null;
            NewFile.DateCreated = DateTime.Now;
            NewFile.DateModified = DateTime.Now;

            //Add the connection
            FileInProject NewConnection = new FileInProject();
            NewConnection.ProjectFile = NewFile;
            Project TheProj = DB.Projects.Where(x => x.ID == Model.ProjectID).FirstOrDefault();
            NewConnection.FileProject = TheProj;
           
            DB.Files.Add(NewFile);
            DB.FilesInProjects.Add(NewConnection);
            DB.SaveChanges();
            return true;
        }

        public bool SaveFile(int ProjectID, int FileID, string NewText)
        {
            File ToSave = GetFileByID(FileID); //DB.Files.Find(FileID);
            ToSave.Location = NewText;
            ToSave.DateModified = DateTime.Now;

            Project TheProject = DB.Projects.Where(x => x.ID == ProjectID).FirstOrDefault(); //Find(ProjectID);
            TheProject.DateModified = DateTime.Now;
            if (DB.SaveChanges() == 0)
            {
                return false;
            }
            return true;
        }

        public bool MoveFile(int ProjectID, int FileID, int? NewFolderID)
        {
            File FileMove = DB.Files.Find(FileID);
            Project TheProject = DB.Projects.Find(ProjectID);
            if (NewFolderID == null)
            {
                var ForceLoad = FileMove.FolderStructure;
                FileMove.FolderStructure = null;
            }
            else
            {
                Folder NewFolder = DB.Folders.Find(NewFolderID);
                NewFolder.DateModified = DateTime.Now;
                FileMove.FolderStructure = NewFolder;
            }
            FileMove.DateModified = DateTime.Now;
            TheProject.DateModified = DateTime.Now;

            if (DB.SaveChanges() == 0)
            {
                return false;
            }
            return true;
        }

        public File ChangeFileName(int ProjectID, int FileID, string NewName)
        {
            File ToRename = DB.Files.Find(FileID);
            
            ToRename.DateModified = DateTime.Now;
            if (NewName.Contains('.'))
            {
                string Type = NewName.Substring(NewName.LastIndexOf('.') + 1);
                ToRename.Type = Type;
                string Name = NewName.Substring(0, NewName.LastIndexOf('.'));
                ToRename.Name = Name;
            }
            else
            {
                ToRename.Name = NewName;
            }

            Project TheProject = DB.Projects.Find(ProjectID);
            TheProject.DateModified = DateTime.Now;
            DB.SaveChanges();
            return ToRename;
        }

        public File GetFileByID(int? ID)
        {
            return DB.Files.Where(x => x.ID == ID).FirstOrDefault(); //Find(ID);
        }

        public List<string> GetTypeList()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Lists/FileTypes.txt");
            List<string> listinn = new List<string>(System.IO.File.ReadLines(path).ToList());
            return listinn;
        }
        public bool DeleteFile(int? ID)
        {
            File ToDelete = GetFileByID(ID);
            if(ToDelete == null)
            {
                return false;
            }
            FileInProject TheConnection = DB.FilesInProjects.Where(x => x.ProjectFile.ID == ToDelete.ID).FirstOrDefault();
            DB.FilesInProjects.Remove(TheConnection);
            DB.Files.Remove(ToDelete);
            
            if (DB.SaveChanges() == 0)
            {
                return false;
            }
            return true;
        }
    }
}