using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class FileService
    {
        private ApplicationDbContext DB = new ApplicationDbContext();

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

        //Checka a USERNAME
        public void AddFile(AddFilesViewModel Model)
        {
           // UserInfo Owner = DB.UsersInfo.Where(x => x.Username == Username).SingleOrDefault();

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
        }

        public void SaveFile(int FileID, string NewText)
        {
            File Tmp = DB.Files.Find(FileID);
            Tmp.Location = NewText;
            DB.SaveChanges();
        }

        public File GetFileByID(int? ID)
        {
            return DB.Files.Find(ID);
        }

        public List<string> GetTypeList()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Lists/FileTypes.txt");
            List<string> listinn = new List<string>(System.IO.File.ReadLines(path).ToList());
            return listinn;
        }
        public void DeleteFile(int? ID)
        {
            File ToDelete = GetFileByID(ID);
            FileInProject TheConnection = DB.FilesInProjects.Where(x => x.ProjectFile == ToDelete).FirstOrDefault();
            DB.FilesInProjects.Remove(TheConnection);
            DB.Files.Remove(ToDelete);
            DB.SaveChanges();
        }

    }
}