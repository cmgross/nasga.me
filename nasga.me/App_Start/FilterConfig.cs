using System.Web;
using System.Web.Mvc;

namespace nasga.me
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //Commented out so that AppHarbor can log errors. The below code supresses them.
            //filters.Add(new HandleErrorAttribute());
        }
    }
}
