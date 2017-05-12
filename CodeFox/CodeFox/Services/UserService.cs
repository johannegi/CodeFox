using CodeFox.Models;
using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CodeFox.Services
{
    public class UserService
    {
        private readonly IAppDataContext db;

        public UserService(IAppDataContext context)
        {
            db = context ?? new ApplicationDbContext();
        }

        //Returns List of User by prefix search
        public List<UserInfo> Autocomplete(string Term, string Username)
        {
            var PossibleOutComes = db.UsersInfo.Where(s => s.Username.ToLower().StartsWith
                                        (Term.ToLower()) && s.Username != Username).Select(w => w).ToList();
            return PossibleOutComes;
        }

        //Returns a List of Users that have specific project shared with them
        public List<UserInfo> GetSharedUsersFromProject(int? ProjectID)
        {
            List<UserInfo> SharedUsers = new List<UserInfo>();       //Get all share connections between specific project and users
            List<ProjectShare> PShare = db.ProjectShares.Where(x => x.ShareProject.ID == ProjectID).ToList();
            if (PShare != null) 
            {
                foreach (var item in PShare) //Loops through all share connections in the list
                {
                    if(item.ShareUser != null) //Adds all Users in connections to List wich is returned
                    {
                        UserInfo tmp = db.UsersInfo.Where(x => x.ID == item.ShareUser.ID).SingleOrDefault();
                        SharedUsers.Add(tmp);
                    }                  
                }
            }
            return SharedUsers;
        }
        public UserInfo GetUserByUsername(string Username)
        {
            UserInfo tmp = db.UsersInfo.Where(x => x.Username == Username).FirstOrDefault();
            return tmp;
        }
        public List<string> GetCountryList()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Lists/Countries.txt");
            List<string> listinn = new List<string>(System.IO.File.ReadLines(path).ToList());
            return listinn;
        }
        public void EditUser(UserInfo User)
        {
            UserInfo ToEdit = db.UsersInfo.Find(User.ID);
            ToEdit.Name = User.Name;
            ToEdit.Email = User.Email;
            ToEdit.Country = User.Country;
            db.SaveChanges();
        }
    }
}