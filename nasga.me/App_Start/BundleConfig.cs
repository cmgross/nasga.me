﻿using System.Web;
using System.Web.Optimization;

namespace nasga.me
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        //TODO use cdn links for everything!!!!!
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/allscripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/scripts.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootstrap3-typeahead.min.js"
                        ));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //"~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryblockUI").Include(
            //"~/Scripts/jquery.blockUI.js"));

            //bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
            //"~/Scripts/scripts.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"
                      ));

            BundleTable.EnableOptimizations = false; //set to true on deployment
        }
    }
}
