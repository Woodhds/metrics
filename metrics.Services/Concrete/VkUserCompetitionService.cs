using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace metrics.Services.Concrete
{
    public class VkUserCompetitionService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        private readonly IVkUserService _vkUserService;
        private readonly ILogger<VkUserCompetitionService> _logger;

        public VkUserCompetitionService(IVkClient vkClient, IVkUserService vkUserService,
            ILogger<VkUserCompetitionService> logger)
        {
            _vkClient = vkClient;
            _vkUserService = vkUserService;
            _logger = logger;
        }

        public async Task<List<VkMessage>> Fetch(int page = 1)
        {
            var data = new List<VkMessage>();
            var users = await _vkUserService.SearchAsync(null);
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
                                response.Response.Items.ForEach(e => { e.RepostedFrom = user.Id; });
                                data.AddRange(response.Response.Items);
                            }

                            await Task.Delay(1000);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Error fetching data from vkusercompetition service. User {user.FullName}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return data;
        }
    }
}