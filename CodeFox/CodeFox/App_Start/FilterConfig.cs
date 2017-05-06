using CodeFox.Utilities;
using System.Web;
using System.Web.Mvc;

namespace CodeFox
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
