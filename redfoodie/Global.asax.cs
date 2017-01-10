using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using redfoodie.Models;
using System.Web.Management;

namespace redfoodie
{
    public class MvcApplication : HttpApplication
    {
        protected void Session_Start(object sender, EventArgs e)
        {
            var context = new ApplicationDbContext();
            Session["citiesList"] = context.Cities.ToArray();
        }

        protected void Application_Start()
        {
            new LogEvent("message to myself").Raise();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public class LogEvent : WebRequestErrorEvent
    {
        public LogEvent(string message)
            : base(null, null, 100001, new Exception(message))
        {
        }
    }
}
