using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeFox.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult NotFound()
        {
            ActionResult Result;

            object Model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                Result = View();
            else
                Result = PartialView("_NotFound");

            return Result;
        }
    }
}