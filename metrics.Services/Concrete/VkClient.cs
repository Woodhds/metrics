using metrics.Services.Abstract;
using metrics.Services.Models;
using metrics.Services.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace metrics.Services.Concrete
{
    public class VkClient : BaseHttpClient, IVkClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VKApiUrls urls;
        public VkClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor, IOptions<VKApiUrls> options) : base(httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            urls = options.Value;
        }

        public async Task<VkResponse<List<VkMessage>>> GetReposts(string id)
        {
            var workID = Regex.Match(id.Replace(urls.MainDomain, string.Empty), @"\d+");
            var userid = string.Empty;
            var owner = string.Empty;
            if(!string.IsNullOrEmpty(workID.Value))
            {
                userid = workID.Value;
            } else
            {
                owner = id.Replace(urls.MainDomain, string.Empty);
            }
            var ci = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if(ci == null)
            {
                return new VkResponse<List<VkMessage>>();
            }
            var token = ci.Claims.FirstOrDefault(c => c.Type == Constants.VK_TOKEN_CLAIM)?.Value;
            var @params = new NameValueCollection()
            {
                { "v", Constants.ApiVersion },
                { "access_token", token?.ToString() },
                { "count", 100.ToString() }
            };
            if(!string.IsNullOrEmpty(userid))
            {
                @params.Add("owner_id", userid);
            }
            else
            {
                @params.Add("domain", owner);
            }

            return await GetAsync<VkResponse<List<VkMessage>>>(new Uri(new Uri(urls.Domain), urls.Wall).AbsoluteUri, @params);
        }
    }
}
