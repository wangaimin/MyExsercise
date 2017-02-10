using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Wam.Utility.EntityBasic;

namespace Wam.Utility.Web.Error
{
   public class CustomHandleErrorAttribute:HandleErrorAttribute
    {

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            filterContext.Result = BuildResult(filterContext);
            filterContext.ExceptionHandled = true;

            base.OnException(filterContext);
        }


        private ActionResult BuildResult(ExceptionContext filterContext) {
           HttpRequestBase request= filterContext.RequestContext.HttpContext.Request;
            if (request.IsAjaxRequest())
            {
                return BuildAjaxJsonActionResult(filterContext);
            }

            return BuildWebPageActionResult(filterContext);
           
        }



        protected  ActionResult BuildWebPageActionResult(ExceptionContext filterContext)
        {
            //action中return view(model)中的model就是ViewData中的Model


            string errorStr = filterContext.Exception.Message;
            Exception exception = new Exception(errorStr);

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();


            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("errorMsg", errorStr);
            vdd.Model = new HandleErrorInfo(filterContext.Exception, controller, action);

            ViewResult vr = new ViewResult()
            {
                ViewName = this.View,
                MasterName = this.Master,
                ViewData = vdd
            };
            return vr;



            //exception.HelpLink = IsBizException(ex) ? "BizEx" : "";

            //var controllerViewData = filterContext.Controller.ViewData;
            //var viewData = new ViewDataDictionary(controllerViewData)
            //{
            //    Model = new HandleErrorInfo(exception, controller, action)
            //};s

            //ViewResult viewResult = new ViewResult
            //{
            //    ViewName = this.View,
            //    MasterName = this.Master,

            //  //  ViewData = viewData,
            //    TempData = filterContext.Controller.TempData
            //};

            //return viewResult;
        }


        protected ActionResult BuildAjaxJsonActionResult(ExceptionContext filterContext) {

            string errorStr = filterContext.Exception.Message;

            JsonResult jsonResult = new JsonResult() {
                Data=new AjaxResult() { Success=false,Msg= errorStr },
                 JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };

            return jsonResult;
        }
    }
}
