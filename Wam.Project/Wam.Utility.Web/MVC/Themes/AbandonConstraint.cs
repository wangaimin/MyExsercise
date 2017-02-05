using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Wam.Utility.Web.MVC.Themes
{
   public class AbandonConstraint: IRouteConstraint
    {

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {

            string browser=  httpContext.Request.Browser.Browser.ToString();
            if (browser=="IE")
            {
                return false;
            }


           // string id = values[parameterName].ToString();
            //可判断参数类型


            return true;
        }
    }
}
