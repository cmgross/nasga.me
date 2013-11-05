using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using nasga.me.Models;
using ServiceStack.Mvc;

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


        [HttpGet] //var results = SearchDAL.SearchStarTrekCharacters(term); //query results here
        public JsonResult GetNames(string term)
        {
            // A list of names to mimic results from a database
            List<string> nameList = new List<string>
            {
                "Jonathan", "Lisa", "Jordan", "Tyler", "Susan", "Brandon", "Clayton", "Elizabeth", "Jennifer"
            };

            var results = nameList.Where(n =>
                n.StartsWith(term, StringComparison.OrdinalIgnoreCase));

            return new JsonResult
            {
                Data = results.ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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