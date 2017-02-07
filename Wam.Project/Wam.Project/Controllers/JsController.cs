using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wam.Project.Controllers
{
    public class JsController : Controller
    {
        // GET: Js
        /// <summary>
        /// 构造函数的继承
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 非构造函数的继承
        /// </summary>
        /// <returns></returns>
        public ActionResult Index2()
        {
            return View();
        }
    }
}