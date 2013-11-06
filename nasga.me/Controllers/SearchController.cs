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
            return View();
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
                var results = service.GetNames(term, year, athleteClass);
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