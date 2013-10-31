using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nasga.me.Interfaces
{
    public interface IProfileManager
    {
        Dictionary<string, string> GetProfile();
        void UpdateProfile(string firstName, string lastName, string athleteClass);
    }
}
