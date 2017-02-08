using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wam.Utility.EntityBasic;
using Wam.Utility.Web.Auth;
using Wam.Utility.Web.Models;

namespace Wam.PortalBase
{
    [UserAuthorize]
    public class BaseController:Controller
    {
        UserModel userModel = null;
        public BaseController() {



            //从session中取登陆信息
            //已经登陆
            userModel = new UserModel() { ID=1,Age=18, UserName="Gene" };

        }

        protected override void OnActionExecuting(ActionExecutingContext context) {
            //没有登录跳转到登录页，没有权限跳转到没有权限页


            //检查是否登陆
            if (userModel==null)
            {
                context.Result = new RedirectResult("/Home/NoRight?errorMsg=您还没有登陆");
            }

            //检查权限(在UserAuthorize中完成)处理权限逻辑，从数据库中取数据判断



            base.OnActionExecuting(context);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (!(ex is BusinessException ))
                {
                  
                    message = "系统发生异常，请稍后再试。";
                }
                string controller = "";//this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = "";// this.ControllerContext.RouteData.Values["action"].ToString();
                var viewData = new ViewDataDictionary()
                {
                    Model = new HandleErrorInfo(ex, controller, action)
                };
                new ViewResult
                {
                    ViewName = "Error404",
                    ViewData = viewData
                }.ExecuteResult(this.ControllerContext);
            }
        }


    }
}
