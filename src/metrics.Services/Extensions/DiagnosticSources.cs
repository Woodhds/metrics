using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using metrics.Authentication.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace metrics.Services.Extensions
{
    public class VkRequestState
    {
        public VkRequestState(IAuthenticatedUserProvider authenticatedUserProvider, string url)
        {
            UserId = authenticatedUserProvider.GetUser().Id;

            ParseQuery(url);
        }

        public int UserId { get; set; }
        public string Method { get; set; }
        public string ApiVersion { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        private void ParseQuery(string url)
        {
            var query = QueryHelpers.ParseQuery(url);
            if (query.TryGetValue("v", out var version))
            {
                ApiVersion = version;
                query.Remove("v");
            }

            var regexp = new Regex(@"(method)/(?<method>\w+\.?\w+)");
            var matches = regexp.Match(url);
            if (matches.Success && matches.Groups.ContainsKey("method"))
            {
                Method = matches.Groups["method"].Value;
            }

            query.Remove("access_token");

            Parameters = query.ToDictionary(x => x.Key, x => x.Value.ToString());
        }
    }
}