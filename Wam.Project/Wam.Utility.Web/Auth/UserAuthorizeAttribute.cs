using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wam.Utility.EntityBasic;

namespace Wam.Utility.Web.Auth
{
    /// <summary>
    /// 用户权限判断
    /// </summary>
    public class UserAuthorizeAttribute: AuthorizeAttribute
    {
        //
        // 摘要:
        //     在过程请求授权时调用。
        //
        // 参数:
        //   filterContext:
        //     筛选器上下文，它封装有关使用 System.Web.Mvc.AuthorizeAttribute 的信息。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     filterContext 参数为 null。
        public override void OnAuthorization(AuthorizationContext filterContext) {

            //读取Cookie登录信息
            //没有登录则跳转到登录页面
            //filterContext.Result = new RedirectResult("/Account/Login");
            //处理权限逻辑，从数据库中取数据判断

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            //没有权限
            //throw new BusinessException("没有权限", 0);
            // throw new Exception("sssas");

            //检查权限

            filterContext.Result = new RedirectResult("/Home/NoRight?errorMsg=没有权限");
        }

    }
}
