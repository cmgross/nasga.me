using System;
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
using ServiceStack.ServiceInterface;
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
            //TODO provide injected AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current)
            using (var service = AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current))
            {
                if (profile.AthleteClass == _configManager.AthleteComboClass)
                {
                    var athletes = profile.AthleteClass.Split('+').Select(athleteClass => new Athlete
                    {
                        FirstName = profile.FirstName, LastName = profile.LastName, Class = athleteClass
                    }).ToList();

                    var athleteResponses = service.Get(athletes);
                    var personalRecordsViewModels = athleteResponses.Select(response => new PersonalRecordsViewModel(response)).ToList();
                    return View("Combo", personalRecordsViewModels);
                }
                var athleteResponse = service.Get(new Athlete{FirstName = profile.FirstName, LastName = profile.LastName, Class = profile.AthleteClass});
                var personalRecordsViewModel = new PersonalRecordsViewModel(athleteResponse);
                return View(personalRecordsViewModel);
            }
        }
    }
}