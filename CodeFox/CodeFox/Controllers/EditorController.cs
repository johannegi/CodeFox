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
        // This controller talks to all the services, so we have an instance of each one.
        private ProjectService Pservice = new ProjectService(null);
        private FileService FService = new FileService(null);
        private FolderService FoService = new FolderService(null);
        private UserService UService = new UserService(null);

        // GET: Editor
        public ActionResult Index(int id)
        {
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(id, Username))
            {
                throw new ArgumentException();
            }
            EditorViewModel EditorView = Pservice.GetEditorViewModel(id);
            return View(EditorView);
        }

        [HttpPost]
        public ActionResult Index(AddFilesViewModel Model)
        {
            return Json("SameName", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTreeJson(int ProjectID)
        {
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(ProjectID, Username))
            {
                throw new ArgumentException();
            }
            EditorViewModel EditorView = Pservice.GetEditorViewModel(ProjectID);
            List<TreeData> Data = new List<TreeData>();
            TreeData Root = new TreeData();
            Root.id = "Project";
            Root.parent = "#";
            Root.text = EditorView.Name;
            Root.type = "root";
            Data.Add(Root);
            foreach (var Item in EditorView.Folders)
            {
                dynamic Folder;
                if (Item.FolderStructure == null)
                {
                    Folder = "Project";
                }
                else
                {
                    Folder = Convert.ToString(Item.FolderStructure.ID);
                }
                TreeData Tmp = new TreeData();
                Tmp.id = Convert.ToString(Item.ID);
                Tmp.parent = Folder;
                Tmp.text = Item.Name;
                Tmp.type = "default";
                Data.Add(Tmp);
            }

            TreeData ReadMe = new TreeData();
            ReadMe.id = Convert.ToString(EditorView.ReadMe.ID);
            ReadMe.parent = "Project";
            ReadMe.text = EditorView.ReadMe.Name + "." + EditorView.ReadMe.Type;
            ReadMe.type = "ReadMe";
            Data.Add(ReadMe);
            foreach (var Item in EditorView.Files)
            {
                dynamic Folder;
                if (Item.FolderStructure == null)
                {
                    Folder = "Project";
                }
                else
                {
                    Folder = Convert.ToString(Item.FolderStructure.ID);
                }
                TreeData Tmp = new TreeData();
                Tmp.id = Convert.ToString(Item.ID);
                Tmp.parent = Folder;
                Tmp.text = Item.Name + '.' + Item.Type;
                Tmp.type = "file";
                Data.Add(Tmp);
            }
            
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFiles(int ID)
        {
            if (!Pservice.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            AddFilesViewModel Model = new AddFilesViewModel();
            Model.TypeList = FService.GetTypeList();
            Model.ProjectID = ID;
            return PartialView("AddFilesModal", Model);
        }

        [HttpPost]
        public ActionResult AddFiles(AddFilesViewModel Model)
        {
            if (!Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            if (ModelState.IsValid)
            {
                
                if(!FService.AddFile(Model))
                {
                    return Json("SameName", JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index", new { id = Model.ProjectID });
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

        public ActionResult AddFolder(int ID)
        {
            if (!Pservice.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            AddFolderViewModel Model = new AddFolderViewModel();
            Model.ProjectID = ID;
            return PartialView("AddFolderModal", Model);
        }

        [HttpPost]
        public ActionResult AddFolder(AddFolderViewModel Model)
        {
            if (!Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            if (ModelState.IsValid)
            {
                FoService.AddFolder(Model);
                return RedirectToAction("Index", new { id = Model.ProjectID });
            }
            return View(Model);
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
            else
            {
                return Json("SameName", JsonRequestBehavior.AllowGet);
            }
            throw new ArgumentException(); ;
        }

        public ActionResult ChangeFolderName(int ProjectID, int FolderID, string NewName)
        {
            var FolderChanged = FoService.ChangeFolderName(ProjectID, FolderID, NewName);
            if (FolderChanged != null)
            {
                return Json(FolderChanged, JsonRequestBehavior.AllowGet);
            }
            throw new ArgumentException();
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
            if(NewFile != null)
            {
                return Json(NewFile, JsonRequestBehavior.AllowGet);
            }
            throw new ArgumentException();

        }

        [HttpGet]
        public ActionResult Share(int id)
        {
            if (!Pservice.CanUserOpenProject(id, User.Identity.Name))
            {
                throw new ArgumentException();
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
                throw new ArgumentException();
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