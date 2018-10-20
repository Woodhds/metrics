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
        private readonly ILogger<VkClient> _logger;
        private object locker = new object();
        public VkClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor, IOptions<VKApiUrls> options,
            ILogger<VkClient> logger) : base(httpClientFactory, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            urls = options.Value;
            _logger = logger;
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

        private T GetVkAsync<T>(string method, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            T result;
            var url = new Uri(new Uri(urls.Domain), method).AbsoluteUri;
            lock (locker)
            {
                result = base.GetAsync<T>(url, @params).GetAwaiter().GetResult();
                Thread.Sleep(500);
            }

            return result;
        }

        private T PostVkAsync<T>(string method, object content, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            T result;
            lock (locker)
            {
                var url = new Uri(new Uri(urls.Domain), method).AbsoluteUri;
                result = base.PostAsync<T>(url, content, @params).GetAwaiter().GetResult();
                Thread.Sleep(500);
            }

            return result;
        }

        public VkResponse<List<VkMessage>> GetReposts(string id, int skip, int take, string search = null)
        {
            var workID = id.Replace(urls.MainDomain, string.Empty);
            var userid = string.Empty;
            var owner = string.Empty;
            if (workID.StartsWith("id"))
            {
                userid = Regex.Match(workID, @"\d+")?.Value;
            }
            else
            {
                owner = workID;
            }

            var @params = new NameValueCollection()
            {
                { "count", take.ToString() },
                { "offset", skip.ToString() },
                { "filter", "owner" }
            };
            if (!string.IsNullOrEmpty(userid))
            {
                @params.Add("owner_id", userid);
            }
            else
            {
                @params.Add("domain", owner);
            }
            string method = urls.Wall;
            if (!string.IsNullOrEmpty(search))
            {
                method = urls.WallSearch;
                @params.Add("query", search);
            }
            return GetVkAsync<VkResponse<List<VkMessage>>>(method, @params);
        }

        public void JoinGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                { "group_id", groupId.ToString() }
            };

            GetVkAsync<VkResponse<int>>(urls.GroupJoin, @params);
        }

        public void Repost(List<VkRepostViewModel> vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
                throw new ArgumentNullException(nameof(vkRepostViewModels));

            var posts = GetById(vkRepostViewModels);
            foreach (var item in posts.Response.Items.Where(c => c.Reposts != null 
                && !c.Reposts.User_reposted))
            {
                try
                {
                    foreach(var group in posts.Response.Groups)
                    {
                        JoinGroup(group.Id);
                    }
                    var @params = new NameValueCollection()
                    {
                        { "object", $"wall{item.owner_id}_{item.id}" }
                    };
                    PostVkAsync<RepostMessageResponse>(urls.Repost, null, @params);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }

        public VkResponse<List<VkMessage>> GetById(List<VkRepostViewModel> vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
            {
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            }
            var @params = new NameValueCollection
            {
                { "posts", string.Join(",", vkRepostViewModels.Select(c => $"{c.Owner_id}_{c.Id}")) },
                { "extended", 1.ToString() }
            };
            return GetVkAsync<VkResponse<List<VkMessage>>>(urls.WallGetById, @params);
        }
    }
}
