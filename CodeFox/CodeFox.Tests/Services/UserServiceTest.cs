using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeFox.Models.Entities;
using CodeFox.Services;
using CodeFox.Tests;
using CodeFox.Models.ViewModels;

namespace CodeFox.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private UserService Service;

        [TestInitialize]
        public void Initialize()
        {
            var MockDb = new MockDatabase();


            var U1 = new UserInfo
            {
                ID = 1,
                Name = "Patti",
                Username = "Patti123"
            };
            MockDb.UsersInfo.Add(U1);

            var U2 = new UserInfo
            {
                ID = 2,
                Name = "Alex",
                Username = "Alex123",
                Country = "Iceland",
                Email = "alex@beggi.is",
                DateCreated = DateTime.Now
            };
            MockDb.UsersInfo.Add(U2);

            var U3 = new UserInfo
            {
                ID = 3,
                Name = "Joey",
                Username = "Joey123"
            };
            MockDb.UsersInfo.Add(U3);

            var U4 = new UserInfo
            {
                ID = 4,
                Name = "Nonni",
                Username = "Nonni123"
            };
            MockDb.UsersInfo.Add(U4);

            var P1 = new Project
            {
                ID = 1,
                Owner = U1
            };
            MockDb.Projects.Add(P1);

            var P2 = new Project
            {
                ID = 2,
                Owner = U1
            };
            MockDb.Projects.Add(P2);

            var P3 = new Project
            {
                ID = 3,
                Owner = U2
            };
            MockDb.Projects.Add(P3);

            var PS1 = new ProjectShare
            {
                ID = 1,
                ShareProject = P1,
                ShareUser = U2
            };
            MockDb.ProjectShares.Add(PS1);

            var PS2 = new ProjectShare
            {
                ID = 2,
                ShareProject = P1,
                ShareUser = U3
            };
            MockDb.ProjectShares.Add(PS2);

            var PS3 = new ProjectShare
            {
                ID = 3,
                ShareProject = P1,
                ShareUser = U4
            };
            MockDb.ProjectShares.Add(PS3);

            var PS4 = new ProjectShare
            {
                ID = 4,
                ShareProject = P3,
                ShareUser = U4
            };
            MockDb.ProjectShares.Add(PS4);

            Service = new UserService(MockDb);
        }

        [TestMethod]
        public void TestGetSharedUsersFromProjectWith3Shares()
        {
            //Arrange
            const int ProjectID = 1;

            //Act
            var result = Service.GetSharedUsersFromProject(ProjectID);

            //Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestGetSharedUsersFromProjectWith0Shares()
        {
            //Arrange
            const int ProjectID = 2;

            //Act
            var result = Service.GetSharedUsersFromProject(ProjectID);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestGetSharedUsersFromProjectWith1Share()
        {
            //Arrange
            const int ProjectID = 3;

            //Act
            var result = Service.GetSharedUsersFromProject(ProjectID);

            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestGetUserByUsername()
        {
            //Arrange
            const string Username = "Nonni123";

            //Act
            var result = Service.GetUserByUsername(Username);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Username, result.Username);
            Assert.AreEqual(4, result.ID);
        }
    }
}
