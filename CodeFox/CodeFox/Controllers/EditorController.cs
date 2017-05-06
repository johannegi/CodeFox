using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using CodeFox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

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
        [HttpGet]
        public ActionResult Share(int? id)
        {
           ShareProjectViewModel Model = new ShareProjectViewModel();
            Model.AllUsers = UService.GetAllUsers(User.Identity.GetUserName());
            Model.SharedWith = UService.GetSharedUsersFromProject(id);
            Model.ShareProject = Pservice.GetProjectFromID(id);
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share(FormCollection collection)
        {
            string Username = collection["AddUsername"].ToString();
            string ProjectIDStr = collection["ProjectID"].ToString();
            int ProjectID = Int32.Parse(ProjectIDStr);

            if(Pservice.AddCollaborator(Username, ProjectID))
            {
                return RedirectToAction("Index", new { id = ProjectID });
            }
            //TODO: implement error logger
            return RedirectToAction("Index", new { id = ProjectID }); //Remove this when implemented
        }

        [HttpPost]
      public ActionResult Autocomplete(string term)
        {
          var AllUsers = UService.GetAllUsers(User.Identity.GetUserName());

            if(term == "")
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            if (term != null)
            {
                var PossibleOutComes = AllUsers.Where(s => s.Username.ToLower().StartsWith
                                     (term.ToLower())).Select(w => w).ToList();

                if (PossibleOutComes == null)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                return Json(PossibleOutComes, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}