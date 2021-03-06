﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YZ.JsonRpc.Client;

namespace Wam.Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {


            string result = Rpc.Call<string>("WamService.UserService.getName", "Gene");

            return View();
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

        public ActionResult Error404(string requestUrl) {

            ViewBag.RequestUrl = requestUrl;
            return View();
        }

        public ActionResult Error500(string requestUrl,string errorMsg)
        {
           
            ViewBag.RequestUrl = requestUrl;
            ViewBag.ErrorMsg = errorMsg;
            return View();
        }

        public ActionResult NoRight(string errorMsg) {
            ViewBag.ErrorMsg = errorMsg;

            return View();
}
    }
}