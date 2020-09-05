using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Events;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using Microsoft.Extensions.Logging;
using metrics.Services.Abstractions;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IVkClient _vkClient;
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQueryProcessor _queryProcessor;

        public MessageController(
            IVkClient vkClient,
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
        public Task<DataSourceResponseModel> GetData([FromQuery]MessageSearchQuery query)
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
                    Reposts = reposts,
                    UserId = _httpContextAccessor.HttpContext.User.Identity.GetUserId()
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
        public IActionResult Like([FromQuery] VkRepostViewModel model)
        {
            try
            {
                _vkClient.Like(model);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("type")]
        public async Task<IActionResult> SetType([FromBody] SetMessageTypeEvent @event)
        {
            await _messageBroker.SendAsync(@event);
            return Ok();
        }
    }
}