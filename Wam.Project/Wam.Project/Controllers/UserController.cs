using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wam.Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var result= YZ.JsonRpc.Client.Rpc.Call<User>("APIService.SupplierService.GetMyLibrarySupplierList");
            return View();
        }
    }
}