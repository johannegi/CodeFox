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
        private ProjectService PService = new ProjectService(null);

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
                if (!PService.CreateProject(Model, Username))
                {
                    return RedirectToAction("Index");
                }
                //return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return View(Model);
        }

        public ActionResult Delete(int? ID)
        {
            Project Model = PService.GetProjectFromID(ID);
            string Username = User.Identity.Name;
            if (Username == Model.Owner.Username)
            {
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            throw new ArgumentException();
        }

        [HttpPost]
        public ActionResult Delete(int ProjectID)
        {
            Project Model = PService.GetProjectFromID(ProjectID);
            if (Model != null)
            {
                PService.DeleteProject(Model.ID);
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        public FileResult Export(int ID)
        {
            if (!PService.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new Exception();
            }
            string UserTempDirectory = Server.MapPath("~/Content/UsersTemp/") + User.Identity.Name;
            string UserProjectDirectory = UserTempDirectory + "/Project";
            string UserZipDirectory = UserTempDirectory + "/ZipTemp";
            string FileName = PService.GetProjectFromID(ID).Name + ".zip";

            PService.ExportProjectToDirectory(ID, UserProjectDirectory);

            byte[] ZippedProject = PService.GetZippedProject(UserProjectDirectory, UserZipDirectory);

            Directory.Delete(UserTempDirectory, true);

            return File(ZippedProject, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        [HttpPost]
        public JsonResult GetReadMe(int? ProjectID)
        {
            if (ProjectID.HasValue)
            {
                Project Tmp = PService.GetProjectFromID(ProjectID);
                Models.Entities.File ReadMe = Tmp.ReadMe;
                return Json(ReadMe, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        public ActionResult Search(string Term, bool Owned)
        {
            if (Term != null)
            {
                var Found = PService.Search(Term, User.Identity.Name);
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
        }*/

    }
}
