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
        //private ApplicationDbContext DB = new ApplicationDbContext();
        private FileService FService = new FileService(null);

        public void AddFolder(AddFolderViewModel Model)
        {
            Folder NewFolder = new Folder();
            NewFolder.Name = Model.Name;
            NewFolder.ProjectStructure = DB.Projects.Find(Model.ProjectID);
            NewFolder.FolderStructure = null;
            NewFolder.DateCreated = DateTime.Now;
            NewFolder.DateModified = DateTime.Now;
            DB.Folders.Add(NewFolder);
            DB.SaveChanges();
        }

        public void MoveFolder(int ProjectID, int FolderID, int? NewFolderID)
        {
            Folder FolderMove = DB.Folders.Find(FolderID);
            Project TheProject = DB.Projects.Find(ProjectID);
            if (NewFolderID == null)
            {
                var ForceLoad = FolderMove.FolderStructure;
                FolderMove.FolderStructure = null;
            }
            else
            {
                Folder NewFolder = DB.Folders.Find(NewFolderID);
                NewFolder.DateModified = DateTime.Now;
                FolderMove.FolderStructure = NewFolder;
            }
            FolderMove.DateModified = DateTime.Now;
            TheProject.DateModified = DateTime.Now;

            DB.SaveChanges();
        }

        public void DeleteFolder(int FolderID)
        {
            Folder TheFolder = DB.Folders.Find(FolderID);
            Project TheProject = DB.Projects.Find(TheFolder.ProjectStructure.ID);
            List<Folder> AllFolders = DB.Folders.Where(x => x.ProjectStructure.ID == TheProject.ID).ToList();
            foreach(Folder Fold in AllFolders)
            {
                if(Fold.FolderStructure != null && Fold.FolderStructure.ID == FolderID)
                {
                    DeleteFolder(Fold.ID);
                }
            }
            List<File> FilesInFolder = DB.Files.Where(x => x.FolderStructure.ID == FolderID).ToList();
            foreach (var Item in FilesInFolder)
            {
                FileInProject Tmp = DB.FilesInProjects.Where(x => x.ProjectFile.ID == Item.ID).SingleOrDefault();
                DB.FilesInProjects.Remove(Tmp);
                DB.Files.Remove(Item);
            }
            DB.Folders.Remove(TheFolder);
            DB.SaveChanges();
        }
        public void DeleteFolderAndContent(Folder ToDelete)
        {
            List<File> AllFiles = DB.Files.Where(x => x.FolderStructure.ID == ToDelete.ID).ToList();
            foreach(File Item in AllFiles)
            {
                FService.DeleteFile(Item.ID);
            }
        }

        public Folder ChangeFolderName(int ProjectID, int FolderID, string NewName)
        {
            Folder ToRename = DB.Folders.Find(FolderID);
            ToRename.DateModified = DateTime.Now;
            ToRename.Name = NewName;

            Project TheProject = DB.Projects.Find(ProjectID);
            TheProject.DateModified = DateTime.Now;

            DB.SaveChanges();
            return ToRename;
        }

        public void CreateTempProjectFolders(int? ProjectID, string ProjectPath)
        {
            System.IO.Directory.CreateDirectory(ProjectPath);
            var AllFolders = DB.Folders.Where(x => x.ProjectStructure.ID == ProjectID).ToList();
            foreach(Folder Fold in AllFolders)
            {
                System.IO.Directory.CreateDirectory(ProjectPath + GetFolderPath(Fold));
            }
        }

        //Returns path from specific folder to the root of project recursively
        public string GetFolderPath(Folder Folder)
        {
            if (Folder == null)
            {
                return "/";
            }
            return GetFolderPath(Folder.FolderStructure) + "/" + Folder.Name;
        }
        public Folder GetFolderByID(int? FolderID)
        {
            Folder Returner = DB.Folders.Where(x => x.ID == FolderID).FirstOrDefault();
            return Returner;
        }
    }
}