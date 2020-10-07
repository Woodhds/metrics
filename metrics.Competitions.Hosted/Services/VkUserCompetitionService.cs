using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts.Models;
using metrics.Competitions.Abstractions;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace metrics.Competitions.Hosted.Services
{
    public class VkUserCompetitionService : ICompetitionsService
    {
        private readonly IVkService _vkClient;
        private readonly IUserService _vkUserService;
        private readonly ILogger<VkUserCompetitionService> _logger;

        public VkUserCompetitionService(
            IVkService vkClient,
            IUserService vkUserService,
            ILogger<VkUserCompetitionService> logger
        )
        {
            _vkClient = vkClient;
            _vkUserService = vkUserService;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<VkMessageModel>> Fetch(int page = 1)
        {
            var data = new List<VkMessageModel>();
            var users = await _vkUserService.GetAsync(null);
            foreach (var user in users)
            {
                try
                {
                    for (var i = page; i < 4; i++)
                    {
                        try
                        {
                            var response = await _vkClient.GetReposts(user.Id.ToString(), i, 80);
                            if (response?.Response?.Items != null)
                            {
                                var models = response.Response.Items
                                    .Select(f => new VkMessageModel(f, response.Response.Groups)).ToList();
                                models.ForEach(e => { e.RepostedFrom = user.Id; });
                                data.AddRange(models);
                            }

                            await Task.Delay(1000);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,
                                $"Error fetching data from vkusercompetition service. User {user.FullName}");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }

            return data;
        }
    }
}