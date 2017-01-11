using System.Web.Optimization;

namespace redfoodie
{
    public static class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate.js"));
            bundles.Add(new ScriptBundle("~/bundles/unobtrusive-ajax").Include(
                      "~/Scripts/jquery.unobtrusive-ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/load-image").Include("~/Scripts/load-image.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                      "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
                      "~/Scripts/underscore.js"));

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                      "~/Scripts/chosen.jquery.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css", 
                      "~/Content/css/Site.css",
                      "~/Content/chosen.css"));

            bundles.Add(new StyleBundle("~/bundles/notification").Include("~/Content/css/notification.css"));
            bundles.Add(new StyleBundle("~/bundles/invite").Include("~/Content/css/invite.css"));
            bundles.Add(new StyleBundle("~/bundles/settings/css").Include(
                "~/Content/css/settings.css", "~/Content/selectize.bootstrap3.css"));
            bundles.Add(new StyleBundle("~/bundles/view-profile/css").Include(
                "~/Content/css/view-profile.css"));
            bundles.Add(new StyleBundle("~/bundles/restaurant-details/css").Include(
                "~/Content/css/restaurant-details.css"));
            bundles.Add(new ScriptBundle("~/bundles/restaurant-details/js").Include(
                "~/Scripts/utilities.js"));
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/Site.js"));
            bundles.Add(new ScriptBundle("~/bundles/js/noty").Include(
                      "~/Scripts/js/jquery.noty.packaged.js"));
            bundles.Add(new ScriptBundle("~/bundles/selectize/js").Include(
                      "~/Scripts/js/standalone/selectize.js"));

            #if DEBUG   
                BundleTable.EnableOptimizations = false;
            #else
                BundleTable.EnableOptimizations = true;
            #endif
        }
    }
}
