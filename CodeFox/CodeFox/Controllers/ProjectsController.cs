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
            string path = Server.MapPath("~/Content/Lists/ProjectTypes.txt");
            Model.TypeList = new List<string>(System.IO.File.ReadLines(path).ToList());
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

        
    }
}