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

        public ActionResult AddFiles(int? id)
        {
            AddFilesViewModel Model = new AddFilesViewModel();
            Model.TypeList = FService.GetTypeList();
            Model.ProjectID = (int)id;
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFiles(AddFilesViewModel Model)
        {
            if (ModelState.IsValid)
            {

                FService.AddFile(Model);
                return RedirectToAction("Index", new { id = Model.ProjectID });
            }
            return View();
        }

        [HttpPost]
        public void SaveFile(int FileID, string NewText)
        {
            FService.SaveFile(FileID, NewText);
        }

        [HttpPost]
        public ActionResult OpenNewFile(int FileID)
        {
            File NewFile = new File();
            NewFile = FService.GetFileByID(FileID);
            return Json(NewFile, JsonRequestBehavior.AllowGet);
        }

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
        public ActionResult Share(string Username, int ProjectID)
        {
            if(Pservice.AddCollaborator(Username, ProjectID))
            {
                return Json(Username, JsonRequestBehavior.AllowGet);
            }
            return Json("User is already a collaborator", JsonRequestBehavior.AllowGet);
            
            //TODO: implement error logger            
        }

        [HttpPost]
        public ActionResult DeleteShare(string Username, int? ProjectID)
        {
            if(Pservice.RemoveCollaborator(Username, ProjectID))
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
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