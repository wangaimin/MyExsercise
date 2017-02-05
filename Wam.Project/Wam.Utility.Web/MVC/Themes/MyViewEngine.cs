using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Wam.Utility.Web.MVC.Themes
{
  public  class MyViewEngine: RazorViewEngine
    //VirtualPathProviderViewEngine
    {
        public MyViewEngine() {

            AreaViewLocationFormats = new[] {
                   //SubSystem
                   "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                   "~/SubSystem/{2}/Views/Shared/{0}.cshtml",

                    //default
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml",
                    //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };
            AreaMasterLocationFormats = new[]
                                           {
                                            //SubSystem
                                            "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/{1}/{0}.vbhtml",
                                            "~/SubSystem/{2}/Views/Shared/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/Shared/{0}.vbhtml",

                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                            };

            AreaPartialViewLocationFormats = new[]
                                               {
                                            //SubSystem
                                            "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/{1}/{0}.vbhtml",
                                            "~/SubSystem/{2}/Views/Shared/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/Shared/{0}.vbhtml",

                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                    //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                                    //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                                 };
            ViewLocationFormats = new[]
                                   {
                                           
                                            //SubSystem
                                            "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/{1}/{0}.vbhtml",
                                            "~/SubSystem/{2}/Views/Shared/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/Shared/{0}.vbhtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml",
                                            //"~/Views/{1}/{0}.vbhtml",
                                            "~/Views/Shared/{0}.cshtml",
                                            //"~/Views/Shared/{0}.vbhtml",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            
                                            //SubSystem
                                            "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/{1}/{0}.vbhtml",
                                            "~/SubSystem/{2}/Views/Shared/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/Shared/{0}.vbhtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml",
                                            //"~/Views/{1}/{0}.vbhtml",
                                            "~/Views/Shared/{0}.cshtml",
                                            //"~/Views/Shared/{0}.vbhtml"
                                        };

            PartialViewLocationFormats = new[]
                                             {
                                            //SubSystem
                                            "~/SubSystem/{2}/Views/{1}/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/{1}/{0}.vbhtml",
                                            "~/SubSystem/{2}/Views/Shared/{0}.cshtml",
                                            //"~/SubSystem/{2}/Views/Shared/{0}.vbhtml",

                                                //default
                                                "~/Views/{1}/{0}.cshtml",
                                                //"~/Views/{1}/{0}.vbhtml",
                                                "~/Views/Shared/{0}.cshtml",
                                                //"~/Views/Shared/{0}.vbhtml",
                                             };
            FileExtensions = new[] { "cshtml" };

        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {

            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
