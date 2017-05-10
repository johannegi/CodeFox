using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using CodeFox.Services;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ProjectService PService = new ProjectService();

        // GET: Project

        public ActionResult Index()
        {
            string Username = User.Identity.Name;
            return View(PService.GetProjectsViewModel(Username));
        }

        public ActionResult Create()
        {
            CreateProjectViewModel Model = new CreateProjectViewModel();
            Model.TypeList = PService.GetTypeList();
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProjectViewModel Model)
        {
            if (ModelState.IsValid)
            {
                string Username = User.Identity.Name;
                if (PService.CreateProject(Model, Username))
                {
                    return RedirectToAction("Index");
                }
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return View(Model);
        }

        public ActionResult Delete(int? id)
        {
            Project Model = PService.GetProjectFromID(id);
            string Username = User.Identity.Name;
            if (Username == Model.Owner.Username)
            {
                return View(Model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Project Model)
        {
            PService.DeleteProject(Model.ID);
            return RedirectToAction("Index");
        }
        private ApplicationDbContext db = new ApplicationDbContext();

        public FileResult Export(int? ID)
        {
            if (!PService.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new Exception();
            }
            string UserTempDirectory = Server.MapPath("~/Content/UsersTemp/") + User.Identity.Name;
            string UserProjectDirectory = UserTempDirectory + "/Project";
            string UserZipDirectory = UserTempDirectory + "/ZipTemp";
            //Directory.CreateDirectory(UserProjectDirectory);
            Directory.CreateDirectory(UserZipDirectory);
            string fileName = PService.GetProjectFromID(ID).Name + ".zip";

            PService.ExportProjectToTemp(ID, UserProjectDirectory);

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(UserProjectDirectory);
                zip.Save(UserZipDirectory + "/tempProject.zip");
                byte[] fileBytes = System.IO.File.ReadAllBytes(UserZipDirectory + "/tempProject.zip");
                Directory.Delete(UserTempDirectory, true);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }

        public ActionResult GetProject(int? ProjectID)
        {
            if (ProjectID.HasValue)
            {

                var ProjectCool = PService.GetProjectFromID(ProjectID).ReadMe.Location;
                return Json(ProjectCool, JsonRequestBehavior.AllowGet);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string Term)
        {
            if (Term != null && Term != "")
            {
                var Found = PService.Search(Term);
                if(Found != null)
                {
                    return Json(Found, JsonRequestBehavior.AllowGet);
                }               
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Sort(string Method, bool Ascending)
        {
            if(Method != null && Method != "")
            {
                var Sorted = PService.Sorted(Method, Ascending);
                if (Sorted != null)
                {
                    return Json(Sorted, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
