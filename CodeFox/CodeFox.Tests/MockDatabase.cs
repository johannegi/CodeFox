using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeDbSet;
using System.Data.Entity;

using CodeFox.Models;
using CodeFox.Models.Entities;

namespace CodeFox.Tests
{
    /// <summary>
    /// This is an example of how we'd create a fake database by implementing the 
    /// same interface that the BookeStoreEntities class implements.
    /// </summary>
    public class MockDatabase : IAppDataContext
    {
        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        public MockDatabase()
        {
            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
            this.UsersInfo = new InMemoryDbSet<UserInfo>();
            this.Projects = new InMemoryDbSet<Project>();
            this.Files = new InMemoryDbSet<File>();
            this.Folders = new InMemoryDbSet<Folder>();
            this.FilesInProjects = new InMemoryDbSet<FileInProject>();
            this.ProjectOwners = new InMemoryDbSet<ProjectOwner>();
            this.ProjectShares = new InMemoryDbSet<ProjectShare>();
        }

        public IDbSet<UserInfo> UsersInfo { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<File> Files { get; set; }
        public IDbSet<Folder> Folders { get; set; }

        public IDbSet<FileInProject> FilesInProjects { get; set; }
        public IDbSet<ProjectOwner> ProjectOwners { get; set; }
        public IDbSet<ProjectShare> ProjectShares { get; set; }

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;
            //changes += DbSetHelper.IncrementPrimaryKey<Author>(x => x.AuthorId, this.Authors);
            //changes += DbSetHelper.IncrementPrimaryKey<Book>(x => x.BookId, this.Books);

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}