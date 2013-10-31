using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using nasga.me.Models;
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

        [HttpGet]
        public ActionResult Index()
        {
            var profile = new ProfileViewModel(_configManager, _profileManager.GetProfile())
            {
                ProfileError = (string)TempData["ProfileError"] ?? string.Empty
            };
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Profile profile)
        {
            if (!ModelState.IsValid) return View("Error");
            _profileManager.UpdateProfile(profile.FirstName, profile.LastName, profile.AthleteClass);
            return RedirectToAction("Index");
        }
    }
}