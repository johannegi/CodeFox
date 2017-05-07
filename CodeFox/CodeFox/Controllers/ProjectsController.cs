using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using CodeFox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
                PService.CreateProject(Model, Username);
                return RedirectToAction("Index");
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

    }
}