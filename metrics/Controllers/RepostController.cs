using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using Microsoft.Extensions.Logging;
using metrics.Services.Abstractions;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}