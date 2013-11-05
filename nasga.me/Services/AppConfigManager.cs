using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public List<string> Years { get; set; }
        public int ConfigurationExpirationDays { get; set; }
        public string AthleteComboClass { get; set; }
    }
}