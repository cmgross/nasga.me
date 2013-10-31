using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using nasga.me.Models;
using ServiceStack.ServiceClient.Web;

namespace nasga.me.Controllers
{
    public class PersonalRecordsController : Controller
    {
        private readonly IProfileManager _profileManager;
        private readonly IConfigManager _configManager;

        public PersonalRecordsController(IProfileManager profileManager, IConfigManager configManager)
        {
            _profileManager = profileManager;
            _configManager = configManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var profile = new Profile(_configManager, _profileManager.GetProfile());
            if (!profile.IsValid())
            {
                TempData["ProfileError"] = "Please complete your profile to continue.";
                return RedirectToAction("Index", "Profile");
            }


            //TODO else, contact the athlete record service and pull a record, and return it to the view
            //TODO store everything in inches maybe? http://www.dotnetperls.com/feet-inches
            var client = new JsonServiceClient("http://localhost:62886/athlete/");
            string responseJson = client.Get<string>(profile.FirstName + "/" + profile.LastName + "/" + profile.AthleteClass);
            //https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
            return View(profile);
        }
    }
}