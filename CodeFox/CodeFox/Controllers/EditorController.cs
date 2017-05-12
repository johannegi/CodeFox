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
        public ActionResult Index(int ID)
        {
            // We get the current user and check if he can't open the project.
            string Username = User.Identity.Name;
            if (!Pservice.CanUserOpenProject(ID, Username))
            {
                throw new ArgumentException();
            }
            EditorViewModel EditorView = Pservice.GetEditorViewModel(ID);
            return View(EditorView);
        }

        public ActionResult GetTreeJson(int ProjectID)
        {
            // We check if if the user can't open the project.
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
            // We check if if the user can't open the project.
            if (!Pservice.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            //We initilize the ViewModel and send the partial view.
            AddFilesViewModel Model = new AddFilesViewModel();
            Model.TypeList = FService.GetTypeList();
            Model.ProjectID = ID;
            // Javascript then puts it into the modal.
            return PartialView("AddFilesModal", Model);
        }

        [HttpPost]
        public ActionResult AddFiles(AddFilesViewModel Model)
        {
            // We check if if the user can't open the project.
            if (Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                if (ModelState.IsValid)
                {
                    // The AddFile(Model) returns a boolean.
                    if (!FService.AddFile(Model))
                    {
                        // This is the message we send to the Ajax controller,
                        // if the file has the same name, which handles the error message.
                        return Json("SameName", JsonRequestBehavior.AllowGet);
                    }
                    // If succeded we return to Editor
                    return RedirectToAction("Index", new { id = Model.ProjectID });
                }
                else if (Model.Name == null)
                {
                    return Json("EmptyString", JsonRequestBehavior.AllowGet);
                }
            }
            // If the user can't open the project, or modelstate isn't valid we throw
            // an ArgumentException.
            throw new ArgumentException();
        }

        [HttpPost]
        public void MoveFile(int ProjectID, int FileID, int? NewFolderID)
        {
            // We check if if the user can't open the project.
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                //If succesfully returned we return to Javascript code.
                if (!FService.MoveFile(ProjectID, FileID, NewFolderID))
                {
                    throw new Exception();
                }
            }
        }

        [HttpPost]
        public void MoveFolder(int ProjectID, int FolderID, int? NewFolderID)
        {
            // See MoveFolder for explanations.
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
            // We check if the user can open the project.
            if (!Pservice.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            // We initialize the AddFolderViewModel and return a partialview.
            AddFolderViewModel Model = new AddFolderViewModel();
            Model.ProjectID = ID;
            // See JavaScript files for the receiving end.
            return PartialView("AddFolderModal", Model);
        }

        [HttpPost]
        public ActionResult AddFolder(AddFolderViewModel Model)
        {
            if (Pservice.CanUserOpenProject(Model.ProjectID, User.Identity.Name))
            {
                if (ModelState.IsValid)
                {
                    // We add the folder and redirect to the Editor.
                    FoService.AddFolder(Model);
                    return RedirectToAction("Index", new { id = Model.ProjectID });
                }
                else if (Model.Name == null)
                {
                    return Json("EmptyString", JsonRequestBehavior.AllowGet);
                }
            }
            throw new ArgumentException();
        }

        [ValidateInput(false)]
        [HttpPost]
        public void SaveFile(int ProjectID, int FileID, string NewText)
        {
            // We check if the user can open the project.
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                // If we fail saving the file we throw an exception.
                if (!FService.SaveFile(ProjectID, FileID, NewText))
                {
                    throw new Exception();
                }
            }
        }

        public ActionResult ChangeFileName(int ProjectID, int FileID, string NewName)
        {
            // If the user can open the project he can also change file names.
            if(Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                // If the we get the file that was changed we return it.
                var FileChanged = FService.ChangeFileName(ProjectID, FileID, NewName);
                if (FileChanged != null)
                {
                    return Json(FileChanged, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // If we don't get the file we return this to JavaScript code
                    // for error handling.
                    return Json("SameName", JsonRequestBehavior.AllowGet);
                }
            }            
            throw new ArgumentException(); ;
        }

        public ActionResult ChangeFolderName(int ProjectID, int FolderID, string NewName)
        {
            // If the user can open the project he can change the name on folders.
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                // If we don't find the FolderChanged we throw an ArgumentException.
                var FolderChanged = FoService.ChangeFolderName(ProjectID, FolderID, NewName);
                if (FolderChanged != null)
                {
                    // We return the FolderChanged, see JavaScript code for receiving end.
                    return Json(FolderChanged, JsonRequestBehavior.AllowGet);
                }
            }
            throw new ArgumentException();
        }

        [HttpPost]
        public void DeleteFile(int FileID, int ProjectID)
        {
            // If the user can open the project he can delete files.
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FService.DeleteFile(FileID))
                {
                    // If the delete fails we throw an exception.
                    throw new Exception();
                }
            }
        }

        [HttpPost]
        public void DeleteFolder(int FolderID, int ProjectID)
        {
            // If the user can open the project he can delete files.
            if (Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                if (!FoService.DeleteFolder(FolderID))
                {
                    // If the delete fails we throw an exception.
                    throw new Exception();
                }
            }
            
        }

        [HttpPost]
        public ActionResult OpenNewFile(int FileID)
        {
            // We get the file and return the file if we find it.
            File NewFile = new File();
            NewFile = FService.GetFileByID(FileID);
            if(NewFile != null)
            {
                return Json(NewFile, JsonRequestBehavior.AllowGet);
            }
            // Exception thrown in case we don't find the file.
            throw new ArgumentException();

        }

        public ActionResult Share(int ID)
        {
            // If the user can't open the file an exception is thrown.
            if (!Pservice.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            // Initializing the ShareProjectViewModel.
            ShareProjectViewModel Model = new ShareProjectViewModel();
            Model.SharedWith = UService.GetSharedUsersFromProject(ID);
            Model.ShareProject = Pservice.GetProjectFromID(ID);
            return View(Model);
        }

        [HttpPost]
        public ActionResult Share(string Username, int ProjectID)
        {
            // If the user can't open the file an exception is thrown.
            if (!Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            // If the user isn't already a collaborator we add him.
            if (Pservice.AddCollaborator(Username, ProjectID))
            {
                return Json(Username, JsonRequestBehavior.AllowGet);
            }
            return Json("User is already a collaborator", JsonRequestBehavior.AllowGet);     
        }

        [HttpPost]
        public ActionResult DeleteShare(string Username, int ProjectID)
        {
            // If the user can't open the file an exception is thrown.
            if (!Pservice.CanUserOpenProject(ProjectID, User.Identity.Name))
            {
                throw new ArgumentException();
            }
            // We remove the collaborator and update the client with JavaScript.
            if (Pservice.RemoveCollaborator(Username, ProjectID))
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Autocomplete(string Term)
        {
            //If the term is empty or null we just return an empty string.
            if (Term != null && Term != "" )
            {
                // We return the list from userservice.
                return Json(UService.Autocomplete(Term, User.Identity.Name), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}