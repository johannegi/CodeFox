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
            Default.Type = Type;
            Default.Location = "//This is the default file for this project";
            Default.FolderStructure = null;
            Default.DateCreated = DateTime.Now;
            Default.DateModified = DateTime.Now;

            return Default;
        }

        public void SaveFile(int FileID, string NewText)
        {
            File Tmp = DB.Files.Find(FileID);
            Tmp.Location = NewText;
            DB.SaveChanges();
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
        public File GetFileByID(int? ID)
        {
            return DB.Files.Find(ID);
        }

        
    }
}