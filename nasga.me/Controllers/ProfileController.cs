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
            HttpCookie firstNameCookie = HttpContext.Request.Cookies["nasga_me_fname"];
            HttpCookie lastNameCookie = HttpContext.Request.Cookies["nasga_me_lname"];
            HttpCookie athleteClassCookie = HttpContext.Request.Cookies["nasga_me_class"];
            string firstName = firstNameCookie == null ? "" : firstNameCookie.Value;
            string lastName = lastNameCookie == null ? "" : lastNameCookie.Value;
            string athleteClass = athleteClassCookie == null ? "" : athleteClassCookie.Value;
            ProfileViewModel profileViweModel = new ProfileViewModel(firstName, lastName,athleteClass);
            return View(profileViweModel);
        }

        [HttpPost]
        public ActionResult Index(ProfileViewModel profile)
        {
            //todo anti forgery token, if modelstate is valid, etc
            HttpCookie firstNameCookie = new HttpCookie("nasga_me_fname") { Value = profile.FirstName };
            HttpCookie lastNameCookie = new HttpCookie("nasga_me_lname") { Value = profile.LastName };
            HttpCookie athleteClassCookie = new HttpCookie("nasga_me_class") { Value = profile.AthleteClass };

            HttpContext.Response.Cookies.Remove("nasga_me_fname");
            HttpContext.Response.Cookies.Remove("nasga_me_lname");
            HttpContext.Response.Cookies.Remove("nasga_me_class");

            HttpContext.Response.SetCookie(firstNameCookie);
            HttpContext.Response.SetCookie(lastNameCookie);
            HttpContext.Response.SetCookie(athleteClassCookie);

            return RedirectToAction("Index");
        }
    }
}