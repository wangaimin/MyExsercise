using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wam.Utility.Web.MVC.Themes;

namespace Wam.Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Abandon",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                //  constraints:new { httpMethod=new HttpMethodConstraint("GET")},//限制只有GET能访问
                //   constraints: new { httpMethod = new AbandonConstraint() },//自定义限制
                 constraints: new { httpMethod = new AbandonConstraint() },//自定义限制,ip、路由、参数...各种限制

                //   constraints: new { id=@"\d+" },//简单限制参数




                namespaces: new[] { "Wam.Project.Controllers" }
                
            );

            routes.MapRoute(
               name: "WebApplication002",
               url: "WebApplication002/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "WebApplication002.Controllers" }
           );



        }
    }
}
