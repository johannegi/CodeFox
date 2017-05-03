using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeFox.Models.Entities
{
    public class UserInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public DateTime DateCreated { get; set; }
    }
}