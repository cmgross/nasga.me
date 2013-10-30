using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nasga.me.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            //TODO http://stackoverflow.com/questions/10571450/should-servicestack-be-the-service-layer-in-an-mvc-application-or-should-it-call
            //TODO look at this controller for moving messages to viewbag and displaying them.
            return View();
        }
    }
}