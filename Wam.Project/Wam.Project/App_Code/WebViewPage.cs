using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Wam.Project
{
    public abstract class CustomWebViewPage<TModel> : WebViewPage<TModel>
    {

        protected MvcHtmlString BulidJsRef(string path)
        {
            //此处可以为资源文件添加版本号
            return new MvcHtmlString(Scripts.Render(path).ToString());
        }

        protected MvcHtmlString BulidCssRef(string path)
        {
            //此处可以为资源文件添加版本号
            return new MvcHtmlString(Styles.Render(path).ToString());
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}