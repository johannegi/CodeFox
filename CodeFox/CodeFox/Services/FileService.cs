using CodeFox.Models;
using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class FileService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Models.Entities.File CreateReadMe(string Text, string ProjectName)
        {
            Models.Entities.File ReadMe = new Models.Entities.File();
            ReadMe.Name = "ReadMe";
            ReadMe.Type = "txt";
            ReadMe.Location = "";
            ReadMe.FolderStructure = null;
            ReadMe.DateCreated = DateTime.Now;
            ReadMe.DateModified = DateTime.Now;

            db.Files.Add(ReadMe);
            db.SaveChanges();
            
            string ProjectFolder = HttpContext.Current.Server.MapPath("~/App_Data/" + ProjectName);
            string FileName = "ReadMe.txt";
            string FullPath = Path.Combine(ProjectFolder, FileName);
            System.IO.FileInfo file = new System.IO.FileInfo(FullPath);
            file.Directory.Create();
            System.IO.File.WriteAllText(file.FullName, Text);

            return ReadMe;
        }
    }
}