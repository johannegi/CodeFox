using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeFox.Models.Entities;
using CodeFox.Services;
using CodeFox.Tests;

namespace CodeFox.Tests.Services
{
    [TestClass]
    public class FolderServiceTest
    {
        private FolderService service;

        [TestInitialize]
        public void Initialize()
        {
            var MockDb = new MockDatabase();

            var P1 = new Project
            {
                ID = 1,
                Name = "TheProject"
            };
            MockDb.Projects.Add(P1);
            var F1 = new Folder
            {
                ID = 1,
                Name = "root",
                FolderStructure = null,
                ProjectStructure = P1
            };
            MockDb.Folders.Add(F1);
            var F2 = new Folder
            {
                ID = 2,
                Name = "F1Child",
                FolderStructure = F1,
                ProjectStructure = P1
            };
            MockDb.Folders.Add(F2);

            service = new FolderService(MockDb);
        }

        [TestMethod]
        public void TestFolderConnection()
        {
            
            Folder Fold = service.GetFolderByID(2);

            //Act
            string result = service.GetFolderPath(Fold);

            //Assert
            Assert.AreEqual("//root/F1Child", result);
        }
    }
}
