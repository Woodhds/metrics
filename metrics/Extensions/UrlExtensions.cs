using Microsoft.AspNetCore.Mvc;

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
