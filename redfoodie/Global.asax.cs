using System;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.IO;
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
            Session["citiesList"] = context.Cities.ToArray();
        }

        protected void Application_Start()
        {
            // This directory calculation neede for AppHarbor
            var dir = Server.MapPath("~");
            var dirBin = Path.Combine(dir, "bin");
            if (Directory.Exists(dirBin))
            {
                dir = dirBin;
            }
            SqlServerTypes.Utilities.LoadNativeAssemblies(dir);

            SqlProviderServices.SqlServerTypesAssemblyName =
                "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            new LogEvent(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()).Raise();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
