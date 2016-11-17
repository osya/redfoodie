using System.Web.Mvc;
using System.Web.Routing;
using redfoodie.Models;

namespace redfoodie
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ShortUrl",
                "{shortUrl}",
                new { controller = "Manage", action = "ViewProfile" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
