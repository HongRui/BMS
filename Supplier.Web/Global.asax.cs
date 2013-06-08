using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SupplierReg.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*alljs}", new { alljs = @".*\.js(/.*)?" });
            routes.IgnoreRoute("{*allcss}", new { allcss = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*allgif}", new { allgif = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*allpng}", new { allpng = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*allswf}", new { allswf = @".*\.swf(/.*)?" });
            routes.IgnoreRoute("{*allcur}", new { allcur = @".*\.cur(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );

        }

        protected void Application_Start()
        {
            string configFileName = Server.MapPath("~/log.log4net");
            System.IO.FileInfo f = new System.IO.FileInfo(configFileName);
            LogHelper.LogHelper.SetConfig(f, "");
            LogHelper.LogHelper.Info("App Start");

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            System.Data.Entity.Database.SetInitializer<OUDAL.OUContext>(null);
        }
    }
}