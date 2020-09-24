using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IVkUserService _vkUserService;
        private readonly IQueryProcessor _queryProcessor;

        public UserController(IVkUserService vkUserService, IQueryProcessor queryProcessor)
        {
            _vkUserService = vkUserService;
            _queryProcessor = queryProcessor;
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