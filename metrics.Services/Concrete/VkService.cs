using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Authentication.Infrastructure;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;

namespace metrics.Services.Concrete
{
    public class VkService : IVkService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
        private readonly IVkWallService _wallService;
        private readonly IVkGroupService _vkGroupService;
        private readonly ILogger<VkService> _logger;

        public VkService(
            IMessageBroker messageBroker,
            IAuthenticatedUserProvider authenticatedUserProvider,
            IVkWallService wallService,
            IVkGroupService vkGroupService,
            ILogger<VkService> logger
        )
        {
            _messageBroker = messageBroker;
            _authenticatedUserProvider = authenticatedUserProvider;
            _wallService = wallService;
            _vkGroupService = vkGroupService;
            _logger = logger;
        }

        public async Task<VkResponse<List<VkMessage>>> GetReposts(string id, int page, int take, string? search = null)
        {
            var data = await _wallService.WallSearch(id, page, take, search);

            var reposts = data.Response.Items
                .OrderByDescending(c => c.Date)
                .Where(c => c.CopyHistory != null && c.CopyHistory.Count > 0)
                .Select(c => c.CopyHistory.First())
                .Distinct()
                .ToList();

            var count = data.Response.Count;

            var result = reposts.Any()
                ? await _wallService.GetById(reposts
                    .Select(c => new VkRepostViewModel(c.OwnerId, c.Id))
                )
                : new VkResponse<List<VkMessage>>();

            result.Response ??= new VkResponse<List<VkMessage>>.VkResponseItems();

            result.Response.Count = count;
            return result;
        }

        public async Task Repost(List<VkRepostViewModel> vkRepostViewModels)
        {
            if (vkRepostViewModels == null)
                throw new ArgumentNullException(nameof(vkRepostViewModels));
            vkRepostViewModels = vkRepostViewModels.Distinct().ToList();

            var posts = await _wallService.GetById(vkRepostViewModels);
            var reposts = posts.Response.Items.Where(c => c.Reposts != null).ToArray();
            foreach (var group in posts.Response.Groups.Where(c => !c.Is_member))
            {
                try
                {
                    await _vkGroupService.JoinGroup(group.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }

            foreach (var t in reposts)
            {
                try
                {
                    await _wallService.Repost(new VkRepostViewModel(t.OwnerId, t.Id));
                    await _messageBroker.SendAsync(new RepostCreated
                    {
                        Id = t.Id,
                        OwnerId = t.OwnerId,
                        UserId = _authenticatedUserProvider.GetUser().Id
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
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
    }
}