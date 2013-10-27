using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using nasga.me.Models;
using nasga.me.Helpers;

namespace nasga.me.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /Profile/
        [HttpGet]
        public ActionResult Index()
        {
            var profileViweModel = new ProfileViewModel(GetProfileCookies());
            return View(profileViweModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileViewModel profile)
        {
            if (!ModelState.IsValid) return View("Error");
            UpdateProfileCookies(profile);
            return RedirectToAction("Index");
        }

        private Dictionary<string,string> GetProfileCookies()
        {
            var cookieCollection = new Dictionary<string, string>();

            foreach (string key in HttpContext.Request.Cookies.AllKeys)
            {
                string value = Request.Cookies[key] == null ? "" : Request.Cookies[key].Value;
                cookieCollection.Add(key, value);
            }
            return cookieCollection;
        }

        private void UpdateProfileCookies(ProfileViewModel profile)
        {
            var firstNameCookie = new HttpCookie(AppSettingsGet.FirstNameCookie) { Value = profile.FirstName };
            var lastNameCookie = new HttpCookie(AppSettingsGet.LastNameCookie) { Value = profile.LastName };
            var athleteClassCookie = new HttpCookie(AppSettingsGet.AthleteClassCookie) { Value = profile.AthleteClass };
            HttpContext.Response.Cookies.Remove(AppSettingsGet.FirstNameCookie);
            HttpContext.Response.Cookies.Remove(AppSettingsGet.LastNameCookie);
            HttpContext.Response.Cookies.Remove(AppSettingsGet.AthleteClassCookie);
            HttpContext.Response.SetCookie(firstNameCookie);
            HttpContext.Response.SetCookie(lastNameCookie);
            HttpContext.Response.SetCookie(athleteClassCookie);
        }
    }
}