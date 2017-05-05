using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using CodeFox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace CodeFox.Controllers
{
    public class EditorController : Controller
    {
        private ProjectService Pservice = new ProjectService();
        private FileService FService = new FileService();
        private UserService UService = new UserService();

        // GET: Editor
        [Authorize]
        public ActionResult Index(int? id)
        {
            EditorViewModel EdiorView = Pservice.GetEditorViewModel(id);
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(EdiorView, Username))
            {
                return RedirectToAction("Index", "Projects");
            }
            return View(EdiorView);
        }

        public PartialViewResult NewFile(int? id)
        {
            File NewFile = FService.GetFileByID(id);
            return PartialView("~/Views/Shared/_EditorView.cshtml", NewFile);
        }

        //MUNA AÐ BREYTA!!!!
        public ActionResult Share(int? ProjectID)
        {
            ShareProjectViewModel Model = new ShareProjectViewModel();
            Model.AllUsers = UService.GetAllUsers(User.Identity.GetUserName());
            Model.SharedWith = UService.GetSharedUsersFromProject(ProjectID);
            Model.ShareProject = Pservice.GetProjectFromID(ProjectID);
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share(FormCollection collection)
        {
            string Username = collection["AddUsername"];
            if (Username == "Patti")
            {
                //
                string cool = "cool beans";
            }
            return RedirectToAction("Index");
        }

      public ActionResult Autocomplete(string term)
        {
            var AllUsers = UService.GetAllUsers(User.Identity.GetUserName());

            var PossibleOutComes = AllUsers.Where(s => s.Username.ToLower().Contains
                                   (term.ToLower())).Select(w => w).ToList();

            return Json(PossibleOutComes, JsonRequestBehavior.AllowGet);
        }
    }
}