using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.Interfaces;

namespace nasga.me.Services
{
    public class CookieProfileManager : IProfileManager
    {
        private readonly IHttpContextBaseWrapper _httpContext;
        private readonly IConfigManager _configManager;

        public CookieProfileManager(IHttpContextBaseWrapper httpContext, IConfigManager configManager)
        {
            _httpContext = httpContext;
            _configManager = configManager;
        }

        public Dictionary<string, string> GetProfile()
        {
            var cookieCollection = new Dictionary<string, string>();
            var athleteCookie = _httpContext.Cookies[_configManager.AthleteKey];
            if (athleteCookie == null) return cookieCollection;
            foreach (var key in athleteCookie.Values.AllKeys)
                cookieCollection.Add(key, _httpContext.Cookies[_configManager.AthleteKey].Values[key] ?? "");
            return cookieCollection;
        }

        public void UpdateProfile(string firstName, string lastName, string athleteClass)
        {
            var athleteInfo = new HttpCookie(_configManager.AthleteKey);
            athleteInfo.Values[_configManager.AthleteFirstNameKey] = firstName;
            athleteInfo.Values[_configManager.AthleteLastNameKey] = lastName;
            athleteInfo.Values[_configManager.AthleteClassKey] = athleteClass;
            athleteInfo.Expires = DateTime.Now.AddDays(_configManager.ConfigurationExpirationDays); //later change to 30 day expiration in webconfig
            _httpContext.Response.Cookies.Remove(_configManager.AthleteKey);
            _httpContext.Response.SetCookie(athleteInfo);
        }
    }
}