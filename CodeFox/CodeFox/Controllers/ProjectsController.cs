using CodeFox.Models;
using CodeFox.Models.Entities;
using CodeFox.Models.ViewModels;
using CodeFox.Services;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // An instance we use to talk to ProjectService.
        private ProjectService PService = new ProjectService(null);

        // GET: Project

        public ActionResult Index()
        {
            // We get the user and then the ProjectViewModel for the user.
            string Username = User.Identity.Name;
            return View(PService.GetProjectsViewModel(Username));
        }

        public ActionResult Create()
        {
            // Create a new CreateProjectViewModel and initilize it.
            CreateProjectViewModel Model = new CreateProjectViewModel();
            Model.TypeList = PService.GetTypeList();
            return View(Model);
        }

        [HttpPost]      //Creates a project
        public ActionResult Create(CreateProjectViewModel Model)
        {
            // If the Model is valid.
            if (ModelState.IsValid)
            {
                string Username = User.Identity.Name; //Creates project on users account
                if (PService.CreateProject(Model, Username))
                {
                    return RedirectToAction("Index");
                }
                return Json("SameName", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int? ID)
        {
            // We check if the user is the owner of the project.
            Project Model = PService.GetProjectFromID(ID);
            string Username = User.Identity.Name;
            if (Username == Model.Owner.Username)
            {
               // If so we pop up the modal via Javascript.
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            throw new ArgumentException();
        }

        [HttpPost]
        public ActionResult Delete(int ProjectID)
        {
            //We get the project and if we get null we return the view.
            Project Model = PService.GetProjectFromID(ProjectID);
            if (Model != null)
            {
                PService.DeleteProject(Model.ID);
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        //Exports a project with specific ID to client's machine
        public FileResult Export(int ID)
        {
            // We check if the user can open the project.
            if (!PService.CanUserOpenProject(ID, User.Identity.Name))
            {
                throw new Exception();
            }
            //Makes path in server's drive as a string to a folder with clients Username to prevent conflicts
            string UserTempDirectory = Server.MapPath("~/Content/UsersTemp/") + User.Identity.Name;
            string UserProjectDirectory = UserTempDirectory + "/Project"; //directory is made to put all project files and folders in
            string UserZipDirectory = UserTempDirectory + "/ZipTemp";   //directory is made to put zipped file to export to client
            string FileName = PService.GetProjectFromID(ID).Name + ".zip"; //Name of the zipped file

            PService.ExportProjectToDirectory(ID, UserProjectDirectory); //Makes the project in the users project path

            //zippes the project from the project path and it is saved in zip path and saved as an array of bytes
            byte[] ZippedProject = PService.GetZippedProject(UserProjectDirectory, UserZipDirectory); 

            Directory.Delete(UserTempDirectory, true); //Users directory in server used to make project and zipped file is deleted

            //Returns the array of bytes made as a zipped file to client's machine
            return File(ZippedProject, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        [HttpPost]
        public JsonResult GetReadMe(int? ProjectID)
        {
  
            if (ProjectID.HasValue)
            {
                // We get the project and from that we get the ReadMe file,
                // that we show via model in the Project Index.
                Project Tmp = PService.GetProjectFromID(ProjectID);
                Models.Entities.File ReadMe = Tmp.ReadMe;
                return Json(ReadMe, JsonRequestBehavior.AllowGet);
            }
            // We use this for JavaScript
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LeaveProject(int ProjectID)
        {
            // If the user can open this project he can also leave it, unless he is the owner.
            Project CurrentProject = PService.GetProjectFromID(ProjectID);
            var CurrentUser = User.Identity.Name;
            if(PService.CanUserOpenProject(ProjectID, CurrentUser) && CurrentProject.Owner.Username != CurrentUser)
            {
                PService.RemoveCollaborator(User.Identity.Name, ProjectID);
                return RedirectToAction("Index", "Projects");
            }
            throw new ArgumentException();
        }

    }
}
