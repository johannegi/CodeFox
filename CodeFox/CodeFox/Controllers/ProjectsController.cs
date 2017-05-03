using CodeFox.Models.Entities;
using CodeFox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Controllers
{
    public class ProjectsController : Controller
    {
        private ProjectService Service = new ProjectService();

        // GET: Project
        [Authorize]
        public ActionResult Index()
        {
            string Username = User.Identity.Name;
            return View(Service.GetProjectsViewModel(Username));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project NewProject)
        {
            NewProject.DateCreated = DateTime.Now;
            return View();
        }

        public ActionResult Share()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share(Project ShareProject)
        {
            return View();
        }
    }
}