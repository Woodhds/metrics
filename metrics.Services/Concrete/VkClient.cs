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
using System.Threading;

namespace metrics.Services.Concrete
{
    public class VkClient : BaseHttpClient, IVkClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VKApiUrls urls;
        private object locker = new object();
        public VkClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor, IOptions<VKApiUrls> options) : base(httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            urls = options.Value;
        }

        private NameValueCollection AddVkParams(NameValueCollection @params)
        {
            var ci = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (ci == null || @params == null)
            {
                return @params;
            }
            var token = ci.Claims.FirstOrDefault(c => c.Type == Constants.VK_TOKEN_CLAIM)?.Value;
            @params.Add("v", Constants.ApiVersion);
            @params.Add("access_token", token);

            return @params;
        }

        private async Task<T> GetVkAsync<T>(string url, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            T result;
            lock (locker)
            {
                result = base.GetAsync<T>(url, @params).GetAwaiter().GetResult();
                Thread.Sleep(500);
            }

            return result;
        }

        private async Task<T> PostVkAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            T result;
            lock (locker)
            {
                result = base.PostAsync<T>(url, content, @params).GetAwaiter().GetResult();
                Thread.Sleep(500);
            }

            return result;
        }

        public async Task<VkResponse<List<VkMessage>>> GetReposts(string id, int skip, int take, string search = null)
        {
            var workID = id.Replace(urls.MainDomain, string.Empty);
            var userid = string.Empty;
            var owner = string.Empty;
            if(workID.StartsWith("id"))
            {
                userid = Regex.Match(workID, @"\d+")?.Value;
            } else
            {
                owner = workID;
            }
            
            var @params = new NameValueCollection()
            {
                { "count", take.ToString() },
                { "offset", skip.ToString() },
                { "filter", "owner" }
            };
            if(!string.IsNullOrEmpty(userid))
            {
                @params.Add("owner_id", userid);
            }
            else
            {
                @params.Add("domain", owner);
            }
            string url = new Uri(new Uri(urls.Domain), urls.Wall).AbsoluteUri;
            if (!string.IsNullOrEmpty(search))
            {
                url = new Uri(new Uri(urls.Domain), urls.WallSearch).AbsoluteUri;
                @params.Add("query", search);
            }
            return await GetVkAsync<VkResponse<List<VkMessage>>>(url, @params);
        }

        public async Task<RepostMessageResponse> Repost(int owner, int id)
        {
            var @params = new NameValueCollection()
            {
                { "object", $"wall{owner}_{id}" }
            };

            return await PostVkAsync<RepostMessageResponse>(
                new Uri(new Uri(urls.Domain), urls.Repost).AbsoluteUri, null, @params);
        }
    }
}
