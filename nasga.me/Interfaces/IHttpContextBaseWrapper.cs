using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace nasga.me.Interfaces
{
    public interface IHttpContextBaseWrapper
    {
        HttpCookieCollection Cookies { get; }
        HttpResponse Response { get; }
    }
}
