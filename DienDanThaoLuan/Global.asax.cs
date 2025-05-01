using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DienDanThaoLuan
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static bool isFirstRequestLogged = false;

   

        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(@"D:\Logs\diendanthoaoluan-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.Information("Ứng dụng đã khởi động");
        }
        protected void Application_BeginRequest()
        {
            if (!isFirstRequestLogged)
            {
                isFirstRequestLogged = true;
                string ip = GetClientIp();
                Log.Information("IP đầu tiên truy cập vào hệ thống: {IP}", ip);
            }
        }

        private string GetClientIp()
        {
            try
            {
                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] addresses = ip.Split(',');
                    if (addresses.Length > 0)
                    {
                        return addresses[0].Trim();
                    }
                }

                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return "Không xác định";
            }
        }
    }
}
