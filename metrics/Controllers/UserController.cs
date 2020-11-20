using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using metrics.Services.Abstractions;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _vkUserService;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IMessageBroker _messageBroker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            IUserService vkUserService,
            IQueryProcessor queryProcessor,
            IMessageBroker messageBroker,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _vkUserService = vkUserService;
            _queryProcessor = queryProcessor;
            _messageBroker = messageBroker;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public Task<IEnumerable<VkUserModel>> Users(string searchStr = "")
        {
            return _vkUserService.GetAsync(searchStr);
        }

        [HttpGet("search")]
        public Task<IEnumerable<VkUserModel>> Search([FromQuery] SearchUserQuery query)
        {
            return _queryProcessor.ProcessAsync(query);
        }

        [HttpPost]
        public Task<VkUserModel> Add([Required] string userId)
        {
            return _vkUserService.CreateAsync(userId);
        }
    }
}