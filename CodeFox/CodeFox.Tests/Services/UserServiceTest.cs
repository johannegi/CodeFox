using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeFox.Models.Entities;
using CodeFox.Services;
using CodeFox.Tests;

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
                Username = "Patti"
            };
            MockDb.UsersInfo.Add(U1);

            var U2 = new UserInfo
            {
                ID = 2,
                Name = "Alex",
                Username = "Alex"
            };
            MockDb.UsersInfo.Add(U2);

            var U3 = new UserInfo
            {
                ID = 3,
                Name = "Joey",
                Username = "Joey"
            };
            MockDb.UsersInfo.Add(U3);

            var U4 = new UserInfo
            {
                ID = 4,
                Name = "Nonni",
                Username = "Nonni"
            };
            MockDb.UsersInfo.Add(U4);

            var P1 = new Project
            {
                ID = 1,
                Owner = U1
            };
            MockDb.Projects.Add(P1);

            var PS1 = new ProjectShare
            {
                ID = 1,
                ShareProject = P1,
                ShareUser = U2
            };
            MockDb.ProjectShares.Add(PS1);

            var PS2 = new ProjectShare
            {
                ID = 1,
                ShareProject = P1,
                ShareUser = U3
            };
            MockDb.ProjectShares.Add(PS2);

            var PS3 = new ProjectShare
            {
                ID = 1,
                ShareProject = P1,
                ShareUser = U4
            };
            MockDb.ProjectShares.Add(PS3);

            Service = new UserService(MockDb);
        }

        [TestMethod]
        public void TestGetSharedUsersFromProject()
        {
            //Arrange
            const int ProjectID = 1;

            //Act
            var result = Service.GetSharedUsersFromProject(ProjectID);

            //Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestGetUserByUsername()
        {
            //Arrange
            const string Username = "Nonni";

            //Act
            var result = Service.GetUserByUsername(Username);

            //Assert
            Assert.AreEqual(Username, result.Username);
            Assert.AreEqual(4, result.ID);
        }
    }
}
