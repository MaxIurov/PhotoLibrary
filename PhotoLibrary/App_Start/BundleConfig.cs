﻿using System.Web;
using System.Web.Optimization;

namespace PhotoLibrary
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/scripts/MyEnlargeImage.js",
                        "~/Scripts/MyDatePicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/PagedList.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/site.css"));
        }
    }
}
