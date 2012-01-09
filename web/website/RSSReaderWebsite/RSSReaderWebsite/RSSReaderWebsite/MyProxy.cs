using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RSSReaderWebsite.myCoreService;
using System.Net;

namespace RSSReaderWebsite
{
    public class MyProxy
    {
        private MyProxy ()
        {

        }
        private static CoreServiceImpl proxy;
        public static CoreServiceImpl getProxy()
        {
            if (proxy == null)
                proxy = new CoreServiceImpl()
                {
                    PreAuthenticate = true,
                    Credentials = CredentialCache.DefaultCredentials,
                    CookieContainer = new CookieContainer()
                };
            return proxy;
        }
    }
}