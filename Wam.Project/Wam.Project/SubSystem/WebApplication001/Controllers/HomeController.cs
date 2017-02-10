using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wam.Service;
using Wam.Utility.EntityBasic;

namespace WebApplication001.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult index2() {
            User u = new Entity.User();
           var data= UserService.GetUserList();

            return View(data);
        }
        public ActionResult index3()
        {
            var data = UserService.GetUserList();
            int.Parse(data.First().UserName);
            return View(data);
        }

        public ActionResult IndexAjax() {


            return View();
            //return new ViewResult
            //{
            //    ViewName = "Error",
            //    MasterName = "",
            //    ViewData = base.ViewData,
            //    TempData = base.TempData,
            //    ViewEngineCollection = this.ViewEngineCollection
            //};


            //return new ViewResult() {
            //     ViewName= "IndexAjax"
            //     , MasterName=""
                


            //};
        }

        public ActionResult GetData() {

            return Json( new AjaxResult{ Data="数据"});
        }

        public ActionResult GetErrorData()
        {

            int.Parse("a");
            return Json(new AjaxResult { Data = "数据" });
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}