using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class FolderService
    {
        private readonly IAppDataContext DB;
        public FolderService(IAppDataContext context)
        {
            DB = context ?? new ApplicationDbContext();
        }
        
        //Adds a folder to a project
        public void AddFolder(AddFolderViewModel Model)
        {
            Folder NewFolder = new Folder(); //A new folder is created
            NewFolder.Name = Model.Name;        //It gets Model's name and is added to Project with Model's ProjectID
            NewFolder.ProjectStructure = DB.Projects.Where(x => x.ID == Model.ProjectID).FirstOrDefault();
            NewFolder.FolderStructure = null; //It goes to root of project by default
            NewFolder.DateCreated = DateTime.Now; //DateTime attributes are setted
            NewFolder.DateModified = DateTime.Now;
            NewFolder.ProjectStructure.DateModified = DateTime.Now;
            DB.Folders.Add(NewFolder);
            DB.SaveChanges();
        }

        //Moves folder within the project
        public bool MoveFolder(int ProjectID, int FolderID, int? NewFolderID)
        {
            Folder FolderMove = DB.Folders.Find(FolderID);    //gets folder and project ith specific ID
            Project TheProject = DB.Projects.Find(ProjectID);
            if (NewFolderID == null) //If folder to move is moving to root it's FolderStructure is changed to null
            {
                var ForceLoad = FolderMove.FolderStructure;
                FolderMove.FolderStructure = null;
            }
            else    //otherwise when it is going inside folder with ID of NewFolderID
            {      //it's FolderStructure is changed to that folder
                Folder NewFolder = DB.Folders.Find(NewFolderID);
                NewFolder.DateModified = DateTime.Now;
                FolderMove.FolderStructure = NewFolder;
            }   //Date modified is resetted
            FolderMove.DateModified = DateTime.Now;
            TheProject.DateModified = DateTime.Now;

            if (DB.SaveChanges() == 0) //Changes saved
            {
                return false;
            }
            return true;
        }

        //Deletes specific folder and every file and folder in it
        public bool DeleteFolder(int FolderID)
        {
            Folder TheFolder = GetFolderByID(FolderID);    //Get folder with FolderID and its project
            Project TheProject = DB.Projects.Where( x => x.ID == TheFolder.ProjectStructure.ID).FirstOrDefault();
            if (TheFolder == null || TheProject == null)
            {
                return false;
            }
            List<Folder> AllFolders = DB.Folders.Where(x => x.ProjectStructure.ID == TheProject.ID).ToList();
            foreach(Folder Fold in AllFolders) //Loops through all folders in the project and if a folder is
            {                                 //a child of the folder to delete it is added to the function recursively
                if(Fold.FolderStructure != null && Fold.FolderStructure.ID == FolderID)
                {
                    DeleteFolder(Fold.ID);
                }
            }
            List<File> FilesInFolder = DB.Files.Where(x => x.FolderStructure.ID == FolderID).ToList();
            foreach (var Item in FilesInFolder) //loops through all files in folder to delete and
            {                                        //deletes the file and the connection
                FileInProject Tmp = DB.FilesInProjects.Where(x => x.ProjectFile.ID == Item.ID).SingleOrDefault();
                DB.FilesInProjects.Remove(Tmp);
                DB.Files.Remove(Item);
            }
            DB.Folders.Remove(TheFolder); //After folder is empty it is deleted and changes saved
            if (DB.SaveChanges() == 0)
            {
                return false;
            }
            return true;
        }


        public Folder ChangeFolderName(int ProjectID, int FolderID, string NewName)
        {
            Folder ToRename = GetFolderByID(FolderID); 
            ToRename.DateModified = DateTime.Now;     //Changes name on folder with specific ID
            ToRename.Name = NewName;                 //and resets date modified on it and its project

            Project TheProject = DB.Projects.Where( x => x.ID == ProjectID).FirstOrDefault();
            TheProject.DateModified = DateTime.Now;

            DB.SaveChanges();
            return ToRename;
        }

        //Creates project's folderstructure on server's drive on specific path
        public void CreateTempProjectFolders(int? ProjectID, string ProjectPath)
        {
            System.IO.Directory.CreateDirectory(ProjectPath); //root folder created
            var AllFolders = GetAllFoldersInProject(ProjectID);
            foreach(Folder Fold in AllFolders) //Loop through all folders in project to create them
            {
                System.IO.Directory.CreateDirectory(ProjectPath + GetFolderPath(Fold));
            }
        }

        //Returns path from the root to specific folder recursively
        public string GetFolderPath(Folder Folder)
        {
            if (Folder == null)
            {
                return "";
            }
            return GetFolderPath(Folder.FolderStructure) + "/" + Folder.Name;
        }

        //Returns Folder with specific ID
        public Folder GetFolderByID(int? FolderID)
        {
            Folder Returner = DB.Folders.Where(x => x.ID == FolderID).FirstOrDefault();
            return Returner;
        }

        //Returns all folders in specific project as a List
        public List<Folder> GetAllFoldersInProject(int? ProjectID)
        {
            var Returner = DB.Folders.Where(x => x.ProjectStructure.ID == ProjectID).ToList();
            return Returner;
        }
    }
}