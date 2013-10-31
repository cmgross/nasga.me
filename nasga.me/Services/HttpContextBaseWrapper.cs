using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.Interfaces;

namespace nasga.me.Services
{
    public class HttpContextBaseWrapper : IHttpContextBaseWrapper
    {
        public HttpCookieCollection Cookies { get { return HttpContext.Current.Request.Cookies; } }
        public HttpResponse Response { get { return HttpContext.Current.Response; } }
    }
}