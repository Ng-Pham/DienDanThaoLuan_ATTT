using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DienDanThaoLuan.Attributes
{
    public class CustomRequireHttpsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                var request = filterContext.HttpContext.Request;

                var uriBuilder = new UriBuilder(request.Url)
                {
                    Scheme = "https",
                    Port = 44333 // Cổng HTTPS bạn đã bật trong Visual Studio
                };

                filterContext.Result = new RedirectResult(uriBuilder.ToString());
            }

            base.OnActionExecuting(filterContext);
        }
    }
}