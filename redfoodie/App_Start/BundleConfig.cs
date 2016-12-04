﻿using System.Web.Optimization;

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

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                      "~/Scripts/chosen.jquery.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css", 
                      "~/Content/site.css",
                      "~/Content/chosen.css"));

            bundles.Add(new StyleBundle("~/Content/notification").Include("~/Content/notification.css"));
            bundles.Add(new StyleBundle("~/Content/invite").Include("~/Content/invite.css"));
            bundles.Add(new StyleBundle("~/Content/settings/css").Include(
                "~/Content/settings.css", "~/Content/selectize.bootstrap3.css"));
            bundles.Add(new StyleBundle("~/Content/viewprofile/css").Include(
                "~/Content/viewprofile.css"));

            bundles.Add(new ScriptBundle("~/Content/js").Include(
                      "~/Scripts/Site.js"));
            bundles.Add(new ScriptBundle("~/Content/js/noty").Include(
                      "~/Scripts/jquery.noty.packaged.min.js"));
            bundles.Add(new ScriptBundle("~/Content/selectize/js").Include(
                      "~/Scripts/selectize.min.js"));
        }
    }
}
