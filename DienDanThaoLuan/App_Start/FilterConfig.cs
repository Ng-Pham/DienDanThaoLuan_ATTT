using System.Web;
using System.Web.Mvc;
using DienDanThaoLuan.Attributes;
using DienDanThaoLuan.Filters;

namespace DienDanThaoLuan
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomRequireHttpsAttribute());
            filters.Add(new SessionTimeoutAttribute());
        }
    }
}
