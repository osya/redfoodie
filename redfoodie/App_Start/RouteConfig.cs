using System.Web.Mvc;
using System.Web.Routing;

namespace redfoodie
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Restaurannt rule is before ShortUrl rule to be able to /Restaurant url works
            routes.MapRoute(
                "Restauurant",
                "Restaurant",
                new { controller = "Restaurant", action = "Index" }
            );
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
