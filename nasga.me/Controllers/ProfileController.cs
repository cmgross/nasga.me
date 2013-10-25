using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nasga.me.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /Profile/
        //http://stackoverflow.com/questions/9158225/using-cookie-in-asp-net-mvc-c-sharp
        public ActionResult Index()
        {
            return View();
        }
	}
}