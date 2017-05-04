using CodeFox.Models;
using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class UserService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<UserInfo> GetAllUsers()
        {
            List<UserInfo> AllUsers = db.UsersInfo.ToList();
            return AllUsers;
        }

        public List<UserInfo> GetSharedUsersFromProject(int? ProjectID)
        {
            List<UserInfo> SharedUsers = new List<UserInfo>();
            List<ProjectShare> PShare = db.ProjectShares.Where(x => x.ShareProject.ID == ProjectID).ToList();
            if (PShare != null)
            {
                foreach (var item in PShare)
                {
                    UserInfo tmp = db.UsersInfo.Where(x => x.ID == item.ShareUser.ID).SingleOrDefault();
                    SharedUsers.Add(tmp);
                }
            }
            return SharedUsers;
        }
    }
}