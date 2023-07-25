using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HomeShow
{
    public static class Extensions
    {
        public static string Modify(this HttpRequestBase request, string queryName, string Value)
        {
            var QueryString = new NameValueCollection();
            QueryString.Add(request.QueryString);
            QueryString[queryName] = Value;
            string q = String.Join("&",
               QueryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(QueryString[a])));
            return $"{request.Url.Scheme}://{request.Url.Authority}/{request.Url.LocalPath}?{q}";
        }
    }
}
