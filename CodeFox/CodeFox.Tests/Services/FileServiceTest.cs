using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeFox.Services;
using CodeFox.Models.Entities;
using System.Collections.Generic;

namespace CodeFox.Tests.Services
{
    [TestClass]
    public class FileServiceTest
    {
        private FileService Service;

        [TestInitialize]
        public void Initialize()
        {
            var MockDb = new MockDatabase();

            var P1 = new Project
            {
                ID = 1,
                Name = "Project1",
                Type = "C++"
            };
            MockDb.Projects.Add(P1);

            var Fo1 = new Folder
            {
                ID = 1,
                Name = "Folder1",
                ProjectStructure = P1,
            };
            MockDb.Folders.Add(Fo1);

            var F1 = new File
            {
                ID = 1,
                Name = "File1",
                FolderStructure = Fo1,
                Location = "WOW"
            };
            MockDb.Files.Add(F1);

            Service = new FileService(MockDb);
        }

        [TestMethod]
        public void TestGetFileByID()
        {
            const int FileID = 1;

            File TheFile = Service.GetFileByID(FileID);

            Assert.IsNotNull(TheFile);
            Assert.AreEqual(TheFile.ID, 1);
            Assert.AreEqual(TheFile.Location, "WOW");
        }

        [TestMethod]
        public void TestCreateReadMe()
        {
            //Arrange
            string text = "Does it work?";

            //Act
            File NewReadMe = Service.CreateReadMe(text);

            //Assert
            Assert.AreEqual(NewReadMe.Location, text);
            Assert.AreEqual(NewReadMe.Type, "txt");
            Assert.AreEqual(NewReadMe.Name, "ReadMe");
        }

        [TestMethod]
        public void TestCreateDefaultForWebApp()
        {
            //Arrange
            string ProjectType = "Web Application";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "html");
        }

        [TestMethod]
        public void TestCreateDefaultForCpp()
        {
            //Arrange
            string ProjectType = "C++";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "cpp");
        }

        [TestMethod]
        public void TestCreateDefaultForCs()
        {
            //Arrange
            string ProjectType = "C#";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "cs");
        }

        [TestMethod]
        public void TestCreateDefaultForJs()
        {
            //Arrange
            string ProjectType = "JavaScript";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "js");
        }

        [TestMethod]
        public void TestCreateDefaultForJava()
        {
            //Arrange
            string ProjectType = "Java";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "java");
        }

        [TestMethod]
        public void TestCreateDefaultForDifferent()
        {
            //Arrange
            string ProjectType = "Random";

            //Act
            File NewFile = Service.CreateDefaultFile(ProjectType);

            //Assert
            Assert.AreEqual(NewFile.Type, "txt");
        }

        [TestMethod]
        public void TestCreateExtraForWebApp()
        {
            //Arrange and act
            List<File> DefaultExtra = Service.CreateWebApplication();

            //Assert
            Assert.AreEqual(2, DefaultExtra.Count);
            Assert.AreEqual(DefaultExtra[0].Type, "css");
            Assert.AreEqual(DefaultExtra[1].Type, "js");
        }

        [TestMethod]
        public void TestSaveFile()
        {
            //Arrange
            const string NewText = "This is the new text";

            //Act
            Service.SaveFile(1, 1, NewText);
            File ChangedFile = Service.GetFileByID(1);

            //Assert
            Assert.AreEqual(ChangedFile.Location, NewText);
        }

    }
}
