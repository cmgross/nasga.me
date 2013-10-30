using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using nasga.me.Models;
using nasga.me.Helpers;
using nasga.me.Services;

namespace nasga.me.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileManager _profileManager;
        private readonly IConfigManager _configManager;

        public ProfileController(IProfileManager profileManager, IConfigManager configManager)
        {
            _profileManager = profileManager;
            _configManager = configManager;
        }

        public ProfileController(): this(new CookieProfileManager(), new AppConfigManager())
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            var profileViweModel = new ProfileViewModel(_configManager,_profileManager.GetProfile(_configManager));
            return View(profileViweModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileViewModel profile)
        {
            if (!ModelState.IsValid) return View("Error");
            _profileManager.UpdateProfile(_configManager,profile.FirstName, profile.LastName, profile.AthleteClass);
            return RedirectToAction("Index");
        }
    }
}