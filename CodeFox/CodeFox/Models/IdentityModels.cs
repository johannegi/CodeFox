using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CodeFox.Models.Entities;

namespace CodeFox.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public interface IAppDataContext
    {
        IDbSet<UserInfo> UsersInfo { get; set; }
        IDbSet<Project> Projects { get; set; }
        IDbSet<File> Files { get; set; }
        IDbSet<Folder> Folders { get; set; }

        IDbSet<FileInProject> FilesInProjects { get; set; }
        IDbSet<ProjectOwner> ProjectOwners { get; set; }
        IDbSet<ProjectShare> ProjectShares { get; set; }

        int SaveChanges();
        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IAppDataContext
    {
        public IDbSet<UserInfo> UsersInfo { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<File> Files { get; set; }
        public IDbSet<Folder> Folders { get; set; }

        public IDbSet<FileInProject> FilesInProjects { get; set; }
        public IDbSet<ProjectOwner> ProjectOwners { get; set; }
        public IDbSet<ProjectShare> ProjectShares { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}