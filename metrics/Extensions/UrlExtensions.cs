using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace metrics.Extensions
{
    public static class UrlExtensions
    {
        public static string GetLocalUrl(this IUrlHelper helper, string localUrl)
        {
            if (!helper.IsLocalUrl(localUrl))
            {
                return "/";
            }

            return localUrl;
        }
    }
}
