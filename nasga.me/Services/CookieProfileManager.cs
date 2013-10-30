using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.Helpers;
using nasga.me.Interfaces;

namespace nasga.me.Services
{
    public class CookieProfileManager : IProfileManager
    {
        public Dictionary<string, string> GetProfile(IConfigManager configManager)
        {
            var cookieCollection = new Dictionary<string, string>();
            var athleteCookie = HttpContext.Current.Request.Cookies[configManager.AthleteClassKey];
            if (athleteCookie == null) return cookieCollection;
            foreach (var key in athleteCookie.Values.AllKeys)
                cookieCollection.Add(key, HttpContext.Current.Request.Cookies[configManager.AthleteClassKey].Values[key] ?? "");
            return cookieCollection;
        }

        public void UpdateProfile(IConfigManager configManager,string firstName, string lastName, string athleteClass)
        {
            var athleteInfo = new HttpCookie(configManager.AthleteClassKey);
            athleteInfo.Values[configManager.AthleteFirstNameKey] = firstName;
            athleteInfo.Values[configManager.AthleteLastNameKey] = lastName;
            athleteInfo.Values[configManager.AthleteClassKey] = athleteClass;
            athleteInfo.Expires = DateTime.Now.AddDays(configManager.ConfigurationExpirationDays); //later change to 30 day expiration in webconfig
            HttpContext.Current.Response.Cookies.Remove(configManager.AthleteClassKey);
            HttpContext.Current.Response.SetCookie(athleteInfo);
        }
    }
}