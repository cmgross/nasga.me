using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace nasga.me.Helpers
{
    public static class AppSettingsGet
    {
        public static string FirstNameCookie
        {
            get { return ConfigurationManager.AppSettings["FirstNameCookie"]; }
        }
        public static string LastNameCookie
        {
            get { return ConfigurationManager.AppSettings["LastNameCookie"]; }
        }
        public static string AthleteClassCookie
        {
            get { return ConfigurationManager.AppSettings["AthleteClassCookie"]; }
        }

        public static List<string> AthleteClasses
        {
            get { return ConfigurationManager.AppSettings["AthleteClasses"].Split(',').ToList(); }
        }
    }
}