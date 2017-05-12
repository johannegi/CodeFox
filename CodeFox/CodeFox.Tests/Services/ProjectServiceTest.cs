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
            var U1 = new UserInfo
            {
                ID = 1,
                Username = "Alex"
            };
            MockDb.UsersInfo.Add(U1);

            var P1 = new Project
            {
                ID = 1,
                Owner = U1,
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
                Owner = U1,
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
                Owner = U1,
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
        }
    }
}
