using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nasga.me.Interfaces
{
    public interface IProfileManager
    {
        Dictionary<string, string> GetProfile(IConfigManager configManager);
        void UpdateProfile(IConfigManager configManager,string firstName, string lastName, string athleteClass);
    }
}
