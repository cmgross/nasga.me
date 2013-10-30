using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nasga.me.Interfaces
{
    public interface IConfigManager
    {
        string AthleteKey { get; set; }
        string AthleteFirstNameKey { get; set; }
        string AthleteLastNameKey { get; set; }
        string AthleteClassKey { get; set; }
        List<string> AthleteClasses { get; set; }
        int ConfigurationExpirationDays { get; set; }
    }
}
