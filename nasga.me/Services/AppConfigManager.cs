using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.Helpers;
using nasga.me.Interfaces;

namespace nasga.me.Services
{
    public class AppConfigManager : IConfigManager
    {
        public string AthleteKey { get; set; }
        public string AthleteFirstNameKey { get; set; }
        public string AthleteLastNameKey { get; set; }
        public string AthleteClassKey { get; set; }
        public List<string> AthleteClasses { get; set; }
        public int ConfigurationExpirationDays { get; set; }

        public AppConfigManager()
        {
            AthleteKey = AppSettingsGet.AthleteClassKey;
            AthleteFirstNameKey = AppSettingsGet.AthleteFirstNameKey;
            AthleteLastNameKey = AppSettingsGet.AthleteLastNameKey;
            AthleteClassKey = AppSettingsGet.AthleteClassKey;
            AthleteClasses = AppSettingsGet.AthleteClasses;
            ConfigurationExpirationDays = AppSettingsGet.ConfigurationExpirationDays;
        }
    }
}