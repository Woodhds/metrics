using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstract;

namespace metrics.Services.Concrete
{
    public class VkUserCompetitionService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        private readonly IVkUserService _vkUserService;

        public VkUserCompetitionService(IVkClient vkClient, IVkUserService vkUserService)
        {
            _vkClient = vkClient;
            _vkUserService = vkUserService;
        }

        public async Task<List<VkMessage>> Fetch(int page = 1)
        {
            var data = new List<VkMessage>();
            var users = await _vkUserService.SearchAsync(null);
            foreach (var user in users)
            {
                var response = _vkClient.GetReposts(user.Id.ToString(), page, 80);
                if (response?.Response?.Items != null)
                {
                    data.AddRange(response.Response.Items);
                }
            }

            return data;
        }
    }
}