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
    [Authorize]
    public class EditorController : Controller
    {
        private ProjectService Pservice = new ProjectService();
        private FileService FService = new FileService();
        private FolderService FoService = new FolderService();
        private UserService UService = new UserService();

        // GET: Editor
        public ActionResult Index(int? id)
        {
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(id, Username))
            {
                return RedirectToAction("Index", "Projects");
            }
            EditorViewModel EdiorView = Pservice.GetEditorViewModel(id);
            return View(EdiorView);
        }

        public ActionResult AddFiles(int? id)
        {
            if (!Pservice.CanUserOpenProject(id, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            AddFilesViewModel Model = new AddFilesViewModel();
            Model.TypeList = FService.GetTypeList();
            Model.ProjectID = (int)id;
            return View(Model);
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult AddFiles(AddFilesViewModel Model)
        {
            if (!Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            if (ModelState.IsValid)
            {
                
                if(!FService.AddFile(Model))
                {
                    return Json("Hello", JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index", "Editor", new { id = Model.ProjectID });
            }
            return View();
        }

        [HttpPost]
        public void MoveFile(int ProjectID, int FileID, int? NewFolderID)
        {
            FService.MoveFile(ProjectID, FileID, NewFolderID);
        }

        [HttpPost]
        public void MoveFolder(int ProjectID, int FolderID, int? NewFolderID)
        {
            FoService.MoveFolder(ProjectID, FolderID, NewFolderID);
        }

        public ActionResult AddFolder(int id)
        {
            AddFolderViewModel Model = new AddFolderViewModel();
            Model.ProjectID = id;
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFolder(AddFolderViewModel Model)
        {
            if (!Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            if (ModelState.IsValid)
            {
                FoService.AddFolder(Model);
                return RedirectToAction("Index", new { id = Model.ProjectID });
            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public void SaveFile(int ProjectID, int FileID, string NewText)
        {
            FService.SaveFile(ProjectID, FileID, NewText);
        }

        public ActionResult ChangeFileName(int ProjectID, int FileID, string NewName)
        {
            return Json(FService.ChangeFileName(ProjectID, FileID, NewName), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void DeleteFile(int FileID)
        {
            FService.DeleteFile(FileID);
        }

        [HttpPost]
        public void DeleteFolder(int FolderID)
        {
            FoService.DeleteFolder(FolderID);
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
            if (!Pservice.CanUserOpenProject(id, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
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
            if (!Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            if (Pservice.AddCollaborator(Username, ProjectID))
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