using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.Serialization.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Utils;

namespace metrics.Services.Concrete
{
    public class VkService : BaseHttpClient<VkService>, IVkService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public VkService(
            IVkClient httpClientFactory,
            ILogger<VkService> logger,
            IMessageBroker messageBroker,
            IAuthenticatedUserProvider authenticatedUserProvider,
            IJsonSerializer jsonSerializer
        ) : base(
            httpClientFactory, jsonSerializer, logger)
        {
            _messageBroker = messageBroker;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<VkResponse<List<VkMessage>>> GetReposts(string id, int page, int take, string? search = null)
        {
            var data = await WallSearch(id, page, take, search);

            var reposts = data.Response.Items
                .OrderByDescending(c => c.Date)
                .Where(c => c.CopyHistory != null && c.CopyHistory.Count > 0)
                .Select(c => c.CopyHistory.First())
                .Distinct()
                .ToList();

            var count = data.Response.Count;

            var result = reposts.Any()
                ? await GetById(reposts
                    .Select(c => new VkRepostViewModel(c.OwnerId, c.Id))
                )
                : new VkResponse<List<VkMessage>>();

            result.Response ??= new VkResponse<List<VkMessage>>.VkResponseItems();

            result.Response.Count = count;
            return result;
        }

        public Task<VkResponse<List<VkMessage>>> WallSearch(string id, int skip, int take, string? search = null)
        {
            var @params = new NameValueCollection
            {
                {"count", take.ToString()},
                {"offset", ((skip - 1) * take).ToString()},
                {"filter", "owner"},
                {"owner_id", id}
            };
            var method = VkApiUrls.Wall;
            if (!string.IsNullOrEmpty(search))
            {
                method = VkApiUrls.WallSearch;
                @params.Add("query", search);
            }

            return base.GetAsync<VkResponse<List<VkMessage>>>(method, @params);
        }

        public async Task JoinGroup(int groupId, int timeout)
        {
            var @params = new NameValueCollection
            {
                {"group_id", groupId.ToString()}
            };

            await base.GetAsync<SimpleVkResponse<int>>(VkApiUrls.GroupJoin, @params);
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

            foreach (var t in reposts)
            {
                try
                {
                    var @params = new NameValueCollection
                    {
                        {"object", $"wall{t.OwnerId}_{t.Id}"}
                    };
                    await base.PostAsync<SimpleVkResponse<VkRepostMessage>>(VkApiUrls.Repost, null, @params);
                    await _messageBroker.SendAsync(new RepostCreated
                    {
                        Id = t.Id,
                        OwnerId = t.OwnerId,
                        UserId = _authenticatedUserProvider.GetUser().Id
                    });
                    await Task.Delay(timeout * 1000);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
            }

            foreach (var x in vkRepostViewModels.Select(f => $"{f.Id}+{f.OwnerId}")
                .Except(reposts.Select(f => $"{f.Id}+{f.OwnerId}")))
            {
                await _messageBroker.SendAsync(new RepostCreated
                {
                    Id = Convert.ToInt32(x.Split('+')[0]),
                    OwnerId = Convert.ToInt32(x.Split('+')[1]),
                    UserId = _authenticatedUserProvider.GetUser().Id
                });
            }
        }

        public Task<SimpleVkResponse<List<VkUserResponse>>> GetUserInfo(string id)
        {
            var @params = new NameValueCollection
            {
                {"user_ids", id},
                {"fields", "first_name,last_name,photo_50"}
            };

            return base.GetAsync<SimpleVkResponse<List<VkUserResponse>>>(VkApiUrls.UserInfo, @params);
        }

        public Task<VkResponse<List<VkMessage>>> GetById(IEnumerable<VkRepostViewModel>? vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
            {
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            }

            var @params = new NameValueCollection
            {
                {"posts", string.Join(",", vkRepostViewModels.Select(c => $"{c.OwnerId}_{c.Id}"))},
                {"extended", 1.ToString()},
                {"fields", "is_member"}
            };
            return base.GetAsync<VkResponse<List<VkMessage>>>(VkApiUrls.WallGetById, @params);
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
            return base.GetAsync<VkResponse<List<VkGroup>>>(VkApiUrls.Groups, @params);
        }

        public Task LeaveGroup(int groupId)
        {
            var @params = new NameValueCollection
            {
                {"group_id", $"{groupId}"}
            };
            return base.GetAsync<SimpleVkResponse<string>>(VkApiUrls.LeaveGroup, @params);
        }

        public Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model)
        {
            var @params = new NameValueCollection
            {
                {"owner_id", $"{model.OwnerId}"},
                {"item_id", $"{model.Id}"},
                {"type", "post"}
            };
            return base.GetAsync<SimpleVkResponse<VkResponseLikeModel>>(VkApiUrls.Like, @params);
        }

        public Task<VkResponse<IEnumerable<VkUserResponse>>> SearchUserAsync(string search)
        {
            var @params = new NameValueCollection
            {
                {"q", search},
                {"count", "100"},
                {"fields", "photo_50"}
            };

            return base.GetAsync<VkResponse<IEnumerable<VkUserResponse>>>(VkApiUrls.UserSearch, @params);
        }
    }
}