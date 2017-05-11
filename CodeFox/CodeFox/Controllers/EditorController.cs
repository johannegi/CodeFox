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
        public ActionResult Index(int id)
        {
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(id, Username))
            {
                return RedirectToAction("Index", "Projects");
            }
            EditorViewModel EdiorView = Pservice.GetEditorViewModel(id);
            return View(EdiorView);
        }

        public ActionResult AddFiles(int id)
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
                    return Json("SameName", JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index", "Editor", new { id = Model.ProjectID });
            }
            return View();
        }

        [HttpPost]
        public void MoveFile(int ProjectID, int FileID, int? NewFolderID)
        {
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FService.MoveFile(ProjectID, FileID, NewFolderID))
                {
                    throw new Exception();
                }
            }
        }

        [HttpPost]
        public void MoveFolder(int ProjectID, int FolderID, int? NewFolderID)
        {
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FoService.MoveFolder(ProjectID, FolderID, NewFolderID))
                {
                    throw new Exception();
                }
            }
            
        }

        public ActionResult AddFolder(int id)
        {
            if (!Pservice.CanUserOpenProject(id, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            AddFolderViewModel Model = new AddFolderViewModel();
            Model.ProjectID = id;
            return View(Model);
        }

        [HttpPost]
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
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FService.SaveFile(ProjectID, FileID, NewText))
                {
                    throw new Exception();
                }
            }
        }

        public ActionResult ChangeFileName(int ProjectID, int FileID, string NewName)
        {
            var FileChanged = FService.ChangeFileName(ProjectID, FileID, NewName);
            if(FileChanged != null)
            {
                return Json(FileChanged, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Projects");
        }

        public ActionResult ChangeFolderName(int ProjectID, int FolderID, string NewName)
        {
            var FolderChanged = FoService.ChangeFolderName(ProjectID, FolderID, NewName);
            if (FolderChanged != null)
            {
                return Json(FolderChanged, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Projects");
        }

        [HttpPost]
        public void DeleteFile(int FileID, int ProjectID)
        {
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FService.DeleteFile(FileID))
                {
                    throw new Exception();
                }
            }
        }

        [HttpPost]
        public void DeleteFolder(int FolderID, int ProjectID)
        {
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FoService.DeleteFolder(FolderID))
                {
                    throw new Exception();
                }
            }
            
        }

        [HttpPost]
        public ActionResult OpenNewFile(int FileID)
        {
            File NewFile = new File();
            NewFile = FService.GetFileByID(FileID);
            if(NewFile == null)
            {
                return Json(NewFile, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Projects");

        }

        [HttpGet]
        public ActionResult Share(int id)
        {
            if (!Pservice.CanUserOpenProject(id, User.Identity.Name))
            {
                return RedirectToAction("Index", "Projects");
            }
            ShareProjectViewModel Model = new ShareProjectViewModel();
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
            if (term != null && term != "" )
            {
                return Json(UService.Autocomplete(term, User.Identity.Name), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}