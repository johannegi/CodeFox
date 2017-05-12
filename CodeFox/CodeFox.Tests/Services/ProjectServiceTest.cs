using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeFox.Models.Entities;
using CodeFox.Services;
using CodeFox.Tests;
using CodeFox.Models.ViewModels;
using System.Web.Mvc;

namespace CodeFox.Tests.Services
{
    [TestClass]
    public class ProjectServiceTest
    {
        private ProjectService Service;

        [TestInitialize]
        public void Initialize()
        {
            var MockDb = new MockDatabase();

            var F1 = new File
            {
                ID = 1,
                Name = "File1",
                Location = "WOW"
            };
            MockDb.Files.Add(F1);

            var U1 = new UserInfo
            {
                ID = 1,
                Username = "Alex"
            };
            MockDb.UsersInfo.Add(U1);

            var U2 = new UserInfo
            {
                ID = 2,
                Username = "Jonni"
            };
            MockDb.UsersInfo.Add(U2);

            var P1 = new Project
            {
                ID = 1,
                Name = "Project1",
                ReadMe = F1,
                Type = "C#",
                Owner = U1
            };
            MockDb.Projects.Add(P1);

            var PO1 = new ProjectOwner
            {
                ID = 1,
                Owner = U1,
                OwnerProject = P1
            };
            MockDb.ProjectOwners.Add(PO1);

            var P2 = new Project
            {
                ID = 2,
                Name = "Project2",
                Owner = U1
            };
            MockDb.Projects.Add(P2);

            var PO2 = new ProjectOwner
            {
                ID = 2,
                Owner = U1,
                OwnerProject = P2
            };
            MockDb.ProjectOwners.Add(PO2);

            var P3 = new Project
            {
                ID = 3,
                Name = "Project3",
                Owner = U1
            };
            MockDb.Projects.Add(P3);

            var PO3 = new ProjectOwner
            {
                ID = 3,
                Owner = U1,
                OwnerProject = P3
            };
            MockDb.ProjectOwners.Add(PO3);

            var PVM = new ProjectsViewModel()
            {
                ID = 1,
            };

            Service = new ProjectService(MockDb);

        }

        [TestMethod]
        public void IndexTest()
        {
            // Arrange
            const string Username = "Alex";

            // Act
            ProjectsViewModel result = Service.GetProjectsViewModel(Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Username, Username);
        }

        [TestMethod]
        public void TestCanUserOpenProject()
        {
            //Arrange
            const int ProjectID = 1;
            const string Username = "Alex";

            //Act & ASSert
            Assert.IsTrue(Service.CanUserOpenProject(ProjectID, Username));
        }

        [TestMethod]
        public void TestGetProjectByID()
        {
            //Arrange
            const int ProjectID = 3;

            //Act
            var Result = Service.GetProjectFromID(ProjectID);

            //Assert
            Assert.IsNotNull(Result);
            Assert.AreEqual("Project3", Result.Name);
        }

        [TestMethod]
        public void TestGetEditorViewModel()
        {
            //Arrange
            const int ProjectID = 1;

            //Act
            var Result = Service.GetEditorViewModel(ProjectID);

            //Assert
            Assert.IsNotNull(Result);
            Assert.AreEqual(Result.Type, "C#");
        }

        [TestMethod]
        public void TestAddCollaborator()
        {
            //Arrange
            const int ProjectID = 1;
            const string Username = "Jonni";

            //Act
            Assert.IsTrue(Service.AddCollaborator(Username, ProjectID));
            Assert.IsTrue(Service.CanUserOpenProject(ProjectID, Username));
        }        
    }
}
