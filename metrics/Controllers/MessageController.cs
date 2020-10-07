using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Events;
using metrics.Broker.Abstractions;
using metrics.Broker.Events;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using Microsoft.Extensions.Logging;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IVkLikeService _vkClient;
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQueryProcessor _queryProcessor;

        public MessageController(
            IVkLikeService vkClient,
            ILogger<MessageController> logger,
            IMessageBroker messageBroker,
            IHttpContextAccessor httpContextAccessor, IQueryProcessor queryProcessor)
        {
            _vkClient = vkClient;
            _logger = logger;
            _messageBroker = messageBroker;
            _httpContextAccessor = httpContextAccessor;
            _queryProcessor = queryProcessor;
        }

        [HttpGet("user")]
        public Task<DataSourceResponseModel> GetData([FromQuery] MessageSearchQuery query)
        {
            return _queryProcessor.ProcessAsync(query);
        }

        [HttpPost("repost")]
        public async Task<IActionResult> Repost([FromBody] List<VkRepostViewModel> reposts)
        {
            try
            {
                await _messageBroker.SendAsync(new CreateRepostGroup
                {
                    UserId = _httpContextAccessor.HttpContext.User.Identity.GetUserId(),
                    Reposts = {reposts.Select(f => new VkRepostGroup {Id = f.Id, OwnerId = f.OwnerId})}
                });

                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(false);
            }
        }

        [HttpGet("like")]
        public async Task<IActionResult> Like([FromQuery] VkRepostViewModel model)
        {
            try
            {
                await _vkClient.Like(model);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("type")]
        public async Task<IActionResult> SetType([FromBody] SetMessageType @event)
        {
            await _messageBroker.SendAsync(@event);
            return Ok();
        }
    }
}