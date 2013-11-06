using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using nasga.me.App_Start;
using nasga.me.Interfaces;
using nasga.me.Models;
using ServiceStack.Mvc;
using ServiceStack.WebHost.Endpoints;
using WebGrease.Css.Ast.Animation;

namespace nasga.me.Controllers
{
    public class SearchController : Controller
    {
        private readonly IConfigManager _configManager;

        public SearchController(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var searchViewModel = new SearchViewModel(_configManager);
            return View(searchViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Search search)
        {
            if (!ModelState.IsValid) return View("Error");
            string[] name = search.Name.Split(' ');
            if (name.Length < 1) return View("Error");
            var firstName = name[0];
            var lastName = name[1];
            using (var service = AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current))
            {
                var athleteResponse = service.Get(new Athlete { FirstName = firstName, LastName = lastName, Class = search.AthleteClass });
                var personalRecordsViewModel = new PersonalRecordsViewModel(athleteResponse);
                return View("Results", personalRecordsViewModel);
            }
        }

        [HttpGet]
        public JsonResult GetNames(string term, string year, string athleteClass)
        {
            if (year == string.Empty || athleteClass == string.Empty) return new JsonResult
            {
                Data = string.Empty.ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            using (var service = AppHostBase.ResolveService<AthleteService>(System.Web.HttpContext.Current))
            {
                int athleteYear = int.Parse(year);
                var results = service.GetNames(term, athleteYear, athleteClass);
                return new JsonResult
                {
                    Data = results.ToArray(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ServiceStackJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
    }
}