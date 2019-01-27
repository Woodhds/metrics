using System;
using System.Collections.Specialized;
using System.Linq;

namespace metrics.Services.Helpers
{
    public static class Url
    {
        public static string BuildUrl(this NameValueCollection @params, string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var builder = new UriBuilder(url);
            if (@params != null)
            {
                var query = string.Join("&", 
                    Enumerable.Range(0, @params.Count).Select(c => $"{@params.GetKey(c)}={@params.Get(c)}"));
                builder.Query = query;

            }
            return builder.ToString();
        }
    }
}
