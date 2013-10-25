using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nasga.me.Models;

namespace nasga.me.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /Profile/
        //http://stackoverflow.com/questions/9158225/using-cookie-in-asp-net-mvc-c-sharp
        [HttpGet]
        public ActionResult Index()
        {
            //pull values from cookies and create a viewmodel with that info
            //if cookies are null, pass an empty viewmodel
            var profileViweModel = new ProfileViewModel();
            return View(profileViweModel);
        }

        [HttpPost]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View();
        }
    }
}