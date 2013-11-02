﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using nasga.me.App_Start;
using nasga.me.Interfaces;
using nasga.me.Models;
using ServiceStack.Common.Web;
using ServiceStack.ServiceClient.Web;
using ServiceStack.WebHost.Endpoints;

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
            var athlete = new Athlete
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Class = profile.AthleteClass
            };
            //TODO handle master+pro by having the personal records view model actually have two athletes or another view just for the combo
            //TODO provide injected AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current)
            using (var svc = AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current))
            {
                AthleteResponse athleteResponse = svc.Get(athlete);
                var personalRecordsViewModel = new PersonalRecordsViewModel(athleteResponse);
                return View(personalRecordsViewModel);
            }
        }
    }
}