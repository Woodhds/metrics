using metrics.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Options;
using metrics.Services.Abstractions;
using metrics.Services.Utils;

namespace metrics.Services.Concrete
{
    public class VkClient : BaseHttpClient, IVkClient
    {
        private readonly IVkTokenAccessor _vkTokenAccessor;
        private readonly VkApiUrls _urls;

        public VkClient(IHttpClientFactory httpClientFactory,
            IVkTokenAccessor vkTokenAccessor, IOptions<VkApiUrls> options,
            ILogger<BaseHttpClient> logger) : base(httpClientFactory, logger)
        {
            _vkTokenAccessor = vkTokenAccessor;
            _urls = options.Value;
        }

        private NameValueCollection AddVkParams(NameValueCollection @params)
        {
            if (@params == null)
            {
                @params = new NameValueCollection();
            }

            @params.Add("v", Constants.ApiVersion);
            @params.Add("access_token", _vkTokenAccessor.GetToken());

            return @params;
        }

        private Task<T> GetVkAsync<T>(string method, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            var url = new Uri(new Uri(_urls.Domain), method).AbsoluteUri;
            return base.GetAsync<T>(url, @params);
        }

        private Task<T> PostVkAsync<T>(string method, object content, NameValueCollection @params = null)
        {
            @params = AddVkParams(@params);
            var url = new Uri(new Uri(_urls.Domain), method).AbsoluteUri;
            return base.PostAsync<T>(url, content, @params);
        }

        public async Task<VkResponse<List<VkMessage>>> GetReposts(string id, int page, int take, string search = null)
        {
            var @params = new NameValueCollection()
            {
                {"count", take.ToString()},
                {"offset", ((page - 1) * take).ToString()},
                {"filter", "owner"},
                {"owner_id", id}
            };
            var method = _urls.Wall;
            if (!string.IsNullOrEmpty(search))
            {
                method = _urls.WallSearch;
                @params.Add("query", search);
            }

            var data = await GetVkAsync<VkResponse<List<VkMessage>>>(method, @params);
            var reposts = data.Response.Items
                .OrderByDescending(c => c.Date)
                .Where(c => c.Copy_History != null && c.Copy_History.Count > 0)
                .Select(c => c.Copy_History.First())
                .Distinct()
                .ToList();

            var count = data.Response.Count;

            var result = await GetById(reposts
                .Select(c => new VkRepostViewModel {Id = c.Id, Owner_Id = c.Owner_Id}
                )
            );
            
            if (result.Response == null)
                result.Response = new VkResponse<List<VkMessage>>.VkResponseItems();
            
            result.Response.Count = count;
            return result;
        }

        public async Task JoinGroup(int groupId, int timeout = 0)
        {
            var @params = new NameValueCollection
            {
                {"group_id", groupId.ToString()}
            };

            await GetVkAsync<SimpleVkResponse<bool>>(_urls.GroupJoin, @params);
            await Task.Delay(timeout * 1000);
        }

        public async Task Repost(List<VkRepostViewModel> vkRepostViewModels, int timeout = 0)
        {
            if (vkRepostViewModels == null)
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            vkRepostViewModels = vkRepostViewModels.Distinct().ToList();

            var posts = await GetById(vkRepostViewModels);
            var reposts = posts.Response.Items.Where(c => c.Reposts != null).ToArray();
            foreach (var group in posts.Response.Groups.Where(c => !c.Is_member))
            {
                try
                {
                    await JoinGroup(group.Id, timeout);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
            }

            for (var i = 0; i < reposts.Length; i++)
            {
                try
                {
                    var @params = new NameValueCollection
                    {
                        {"object", $"wall{reposts[i].Owner_Id}_{reposts[i].Id}"}
                    };
                    await PostVkAsync<SimpleVkResponse<VkRepostMessage>>(_urls.Repost, null, @params);
                    await Task.Delay(timeout * 1000);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
            }
        }

        public Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id)
        {
            var @params = new NameValueCollection
            {
                {"user_ids", id},
                {"fields", "first_name,last_name,photo_50"}
            };

            return GetVkAsync<SimpleVkResponse<List<VkUserResponse>>>(_urls.UserInfo, @params);
        }

        public Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel> vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
            {
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            }

            var @params = new NameValueCollection
            {
                {"posts", string.Join(",", vkRepostViewModels.Select(c => $"{c.Owner_Id}_{c.Id}"))},
                {"extended", 1.ToString()},
                {"fields", "is_member"}
            };
            return GetVkAsync<VkResponse<List<VkMessage>>>(_urls.WallGetById, @params);
        }

        public Task<VkResponse<List<VkGroup>>> GetGroups(int count, int offset)
        {
            var @params = new NameValueCollection
            {
                {"count", $"{count}"},
                {"fields", "name, description"},
                {"extended", "1"},
                {"offset", $"{offset}"}
            };
            return GetVkAsync<VkResponse<List<VkGroup>>>(_urls.Groups, @params);
        }

        public Task LeaveGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                {"group_id", $"{groupId}"}
            };
            return GetVkAsync<SimpleVkResponse<string>>(_urls.LeaveGroup, @params);
        }

        public Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model)
        {
            var @params = new NameValueCollection
            {
                {"owner_id", $"{model.Owner_Id}"},
                {"item_id", $"{model.Id}"},
                {"type", "post"}
            };
            return GetVkAsync<SimpleVkResponse<VkResponseLikeModel>>(_urls.Like, @params);
        }
    }
}