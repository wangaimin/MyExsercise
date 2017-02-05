using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication001
{
    public class Web001_AreaRegistration: AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Web001_default",
                "WebApplication001/{controller}/{action}/{id}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    area = "WebApplication001",
                    id = ""
                },
                new[] { "WebApplication001.Controllers" }
            );
        }

        public override string AreaName
        {
            get { return "WebApplication001"; }
        }
    }
}