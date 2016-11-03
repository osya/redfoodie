using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using redfoodie.Models;

namespace redfoodie
{
    public class MvcApplication : HttpApplication
    {
        protected void Session_Start(object sender, EventArgs e)
        {
            var context = new ApplicationDbContext();
            Session["cities"] = context.Cities.ToList();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
