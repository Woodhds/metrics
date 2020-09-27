using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace metrics.Services.Utils.Helpers
{
    public static class Url
    {
        public static string BuildUrl(this NameValueCollection? @params, string url)
        {
            @params ??= new NameValueCollection();

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            foreach (var c in Enumerable.Range(0, @params.Count))
            {
                url = QueryHelpers.AddQueryString(url, @params.GetKey(c), @params.Get(c));
            }

            return url;
        }
    }
}