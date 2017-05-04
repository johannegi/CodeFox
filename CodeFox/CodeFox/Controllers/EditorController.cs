using CodeFox.Models.ViewModels;
using CodeFox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Controllers
{
    public class EditorController : Controller
    {
        private ProjectService Pservice = new ProjectService();

        // GET: Editor
        [Authorize]
        public ActionResult Index(int? id)
        {
            EditorViewModel EdiorView = Pservice.GetEditorViewModel(id);
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(EdiorView, Username))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(EdiorView);
        }
    }
}