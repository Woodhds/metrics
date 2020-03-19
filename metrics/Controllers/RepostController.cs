using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : ControllerBase
    {
        private readonly IVkClient _vkClient;
        private readonly IVkMessageService _vkMessageService;
        private readonly ILogger<RepostController> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepostController(
            IVkClient vkClient,
            ILogger<RepostController> logger,
            IVkMessageService vkMessageService,
            IMessageBroker messageBroker,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _vkClient = vkClient;
            _logger = logger;
            _vkMessageService = vkMessageService;
            _messageBroker = messageBroker;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public async Task<ActionResult<DataSourceResponseModel>> GetData(int page, int pageSize,
            string search = null, string user = null)
        {
            try
            {
                return await _vkMessageService.GetMessages(page, pageSize, search, user);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest();
            }
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpPost("repost")]
        public async Task<IActionResult> Repost(CancellationToken ct, [FromBody] List<VkRepostViewModel> reposts)
        {
            try
            {
                await _messageBroker.PublishAsync(new RepostGroupCreatedEvent
                {
                    Reposts = reposts,
                    UserId = _httpContextAccessor.HttpContext.User.Identity.GetUserId()
                }, ct);

                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(false);
            }
        }

        [Authorize(Policy = "VkPolicy")]
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
    }
}