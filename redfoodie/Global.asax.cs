﻿using System;
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
            Session["citiesList"] = new SelectList(context.Cities, "Id", "Name");
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
