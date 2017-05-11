using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeFox.Models.Entities;
using CodeFox.Services;
using CodeFox.Tests;
using CodeFox.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

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
                Name = "Project1",
            };
            MockDb.Projects.Add(P1);
           /* var P2 = new Project
            {
                ID = 2,
                Name = "Project2"
            };
            MockDb.Projects.Add(P2);*/

            var F1 = new Folder
            {
                ID = 1,
                Name = "1Root",
                FolderStructure = null,
                ProjectStructure = P1
            };
            MockDb.Folders.Add(F1);
            var F2 = new Folder
            {
                ID = 2,
                Name = "11Child",
                FolderStructure = F1,
                ProjectStructure = P1
            };
            MockDb.Folders.Add(F2);
            var F3 = new Folder
            {
                ID = 3,
                Name = "12Child",
                FolderStructure = F1,
                //ProjectStructure = P1
            };
            MockDb.Folders.Add(F3);
            var F4 = new Folder
            {
                ID = 4,
                Name = "111Grandchild",
                FolderStructure = F2,
                //ProjectStructure = P1
            };
            MockDb.Folders.Add(F4);

            service = new FolderService(MockDb);
        }

        [TestMethod]
        public void TestGet111GrandchildFolderPath()
        {
            //Arrange
            Folder Fold = service.GetFolderByID(4);

            //Act
            string result = service.GetFolderPath(Fold);

            //Assert
            Assert.AreEqual("/1Root/11Child/111Grandchild", result);
        }

        [TestMethod]
        public void TestGet12ChildFolderPath()
        {
            //Arrange
            Folder Fold = service.GetFolderByID(3);

            //Act
            string result = service.GetFolderPath(Fold);

            //Assert
            Assert.AreEqual("/1Root/12Child", result);
        }

        [TestMethod]
        public void TestGet11ChildFolderPath()
        {
            //Arrange
            Folder Fold = service.GetFolderByID(2);

            //Act
            string result = service.GetFolderPath(Fold);

            //Assert
            Assert.AreEqual("/1Root/11Child", result);
        }

        [TestMethod]
        public void TestGetRootFolderPath()
        {
            //Arrange
            Folder Fold = service.GetFolderByID(1);

            //Act
            string result = service.GetFolderPath(Fold);

            //Assert
            Assert.AreEqual("/1Root", result);
        }

        [TestMethod]
        public void TestAddFolder()
        {
            //Arrange
            

            //Act
            
            

            Assert.AreEqual(null, null);
        }
    }
}
