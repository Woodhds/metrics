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
using System.Linq;
using System.Threading;

namespace metrics.Services.Concrete
{
    public class VkClient : BaseHttpClient, IVkClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VKApiUrls _urls;
        private object locker = new object();
        public VkClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor, IOptions<VKApiUrls> options,
            ILogger<BaseHttpClient> logger) : base(httpClientFactory, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _urls = options.Value;
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
            var url = new Uri(new Uri(_urls.Domain), method).AbsoluteUri;
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
            Monitor.Enter(locker);

            var url = new Uri(new Uri(_urls.Domain), method).AbsoluteUri;
            result = base.PostAsync<T>(url, content, @params).GetAwaiter().GetResult();
            Thread.Sleep(500);

            Monitor.Exit(locker);

            return result;
        }

        public VkResponse<List<VkMessage>> GetReposts(string id, int page, int take, string search = null)
        {

            var @params = new NameValueCollection()
            {
                { "count", take.ToString() },
                { "offset", ((page - 1) * take).ToString() },
                { "filter", "owner" },
                { "owner_id", id }
            };
            string method = _urls.Wall;
            if (!string.IsNullOrEmpty(search))
            {
                method = _urls.WallSearch;
                @params.Add("query", search);
            }
            var data = GetVkAsync<VkResponse<List<VkMessage>>>(method, @params);
            var reposts = data.Response
                .Items.OrderByDescending(c => c.Date)
                .Where(c => c.Copy_History != null && c.Copy_History.Count > 0).Select(c => c.Copy_History.First())
                .Distinct().ToList();
            var count = data.Response.Count;

            var result = GetById(reposts.Select(c => new VkRepostViewModel() { Id = c.Id, Owner_Id = c.Owner_Id }));
            if (result.Response == null)
                result.Response = new VkResponse<List<VkMessage>>.VkResponseItems();
            result.Response.Count = count;
            return result;
        }

        public void JoinGroup(int groupId, int timeout = 0)
        {
            var @params = new NameValueCollection
            {
                { "group_id", groupId.ToString() }
            };

            GetVkAsync<SimpleVkResponse<bool>>(_urls.GroupJoin, @params);
            Thread.Sleep(timeout * 1000);
        }

        public List<SimpleVkResponse<VkRepostMessage>> Repost(List<VkRepostViewModel> vkRepostViewModels, int timeout = 0)
        {
            if (vkRepostViewModels == null)
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            vkRepostViewModels = vkRepostViewModels.Distinct().ToList();

            var result = new List<SimpleVkResponse<VkRepostMessage>>();

            var posts = GetById(vkRepostViewModels);
            foreach (var group in posts.Response.Groups.Where(c => !c.Is_member))
            {
                try
                {
                    JoinGroup(group.Id, timeout);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
            }
            foreach (var item in posts.Response.Items.Where(c => c.Reposts != null))
            {
                try
                {
                    var @params = new NameValueCollection
                    {
                        { "object", $"wall{item.Owner_Id}_{item.Id}" }
                    };
                    result.Add(PostVkAsync<SimpleVkResponse<VkRepostMessage>>(_urls.Repost, null, @params));
                    Thread.Sleep(timeout * 1000);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
            }

            return result;
        }

        public SimpleVkResponse<List<VkUserResponse>> GetUserInfo(string id)
        {
            var @params = new NameValueCollection
            {
                { "user_ids", id },
                { "fields", "first_name,last_name,photo_50" }
            };

            return GetVkAsync<SimpleVkResponse<List<VkUserResponse>>>(_urls.UserInfo, @params);
        }

        public VkResponse<List<VkMessage>> GetById(IEnumerable<VkRepostViewModel> vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
            {
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            }
            var @params = new NameValueCollection
            {
                { "posts", string.Join(",", vkRepostViewModels.Select(c => $"{c.Owner_Id}_{c.Id}")) },
                { "extended", 1.ToString() },
                { "fields", "is_member" }
            };
            return GetVkAsync<VkResponse<List<VkMessage>>>(_urls.WallGetById, @params);
        }

        public VkResponse<List<VkGroup>> GetGroups(int count, int offset)
        {
            var @params = new NameValueCollection
            {
                { "count", $"{count}" },
                { "fields", "name, description" },
                { "extended", "1" },
                { "offset", $"{offset}" }
            };
            return GetVkAsync<VkResponse<List<VkGroup>>>(_urls.Groups, @params);
        }

        public void LeaveGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                { "group_id", $"{groupId}" }
            };
            GetVkAsync<SimpleVkResponse<string>>(_urls.LeaveGroup, @params);
        }

        public SimpleVkResponse<VkResponseLikeModel> Like(VkRepostViewModel model)
        {
            var @params = new NameValueCollection
            {
                { "owner_id", $"{model.Owner_Id}" },
                { "item_id", $"{model.Id}" },
                { "type", "post" }
            };
            return GetVkAsync<SimpleVkResponse<VkResponseLikeModel>>(_urls.Like, @params);
        }
    }
}
