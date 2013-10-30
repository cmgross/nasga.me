using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace nasga.me.Helpers
{
    public static class AppSettingsGet
    {
        public static string AthleteKeyName
        {
            get { return ConfigurationManager.AppSettings["AthleteCookieName"]; }
        }
        public static string AthleteFirstNameKey
        {
            get { return ConfigurationManager.AppSettings["AthleteFirstNameKey"]; }
        }
        public static string AthleteLastNameKey
        {
            get { return ConfigurationManager.AppSettings["AthleteLastNameKey"]; }
        }
        public static string AthleteClassKey
        {
            get { return ConfigurationManager.AppSettings["AthleteClassKey"]; }
        }

        public static int ConfigurationExpirationDays
        {
            get
            {
                var expirationPeriod = ConfigurationManager.AppSettings["ConfigurationExpirationDays"];
                int cookieExpirationPeriod;
                int.TryParse(expirationPeriod, out cookieExpirationPeriod);
                return cookieExpirationPeriod;
            }
        }

        public static List<string> AthleteClasses
        {
            get { return ConfigurationManager.AppSettings["AthleteClasses"].Split(',').ToList(); }
        }
    }
}