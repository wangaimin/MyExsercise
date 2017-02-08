using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wam.Utility.Web.Models;

namespace Wam.Utility.Web.MVC
{
 public   class SetBaseEntityActionFilter:ActionFilterAttribute
    {
        /// <summary>
        /// 更新、新增、删除、更新时设置基础信息
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);

            //从cookie中获取到登录信息
            UserModel userModel = new Models.UserModel() { UserName = "Gene" };

            foreach (var item in filterContext.ActionParameters)
            {
                var baseModel = item.Value as EntityBasic.EntityBase;
                if (baseModel!=null)
                {
                    baseModel.InUserSysNo = userModel.ID;
                    baseModel.InUserName = userModel.UserName;
                    baseModel.InDate = DateTime.Now;

                    baseModel.EditUserSysNo = userModel.ID;
                    baseModel.EditUserName = userModel.UserName;
                    baseModel.EditDate = DateTime.Now;
                }
            }

        }
    }
}
