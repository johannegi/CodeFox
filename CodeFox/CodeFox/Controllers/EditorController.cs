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
        private ProjectService service = new ProjectService();

        // GET: Editor
        public ActionResult Index(int ProjectID)
        {
            return View(service.GetEditorViewModel(ProjectID));
        }
    }
}