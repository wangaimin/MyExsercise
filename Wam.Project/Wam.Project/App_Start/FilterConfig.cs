using System.Web;
using System.Web.Mvc;
using Wam.Utility.Web.Error;

namespace Wam.Project
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
