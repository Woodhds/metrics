using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using Microsoft.AspNetCore.Authorization;
using metrics.Services.Abstract;
using Microsoft.Extensions.Logging;
using metrics.Models;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : ControllerBase
    {
        private readonly IVkClient _vkClient;
        private readonly IVkMessageService _vkMessageService;
        private readonly ILogger<RepostController> _logger;
        public RepostController(IVkClient vkClient, ILogger<RepostController> logger, IVkMessageService vkMessageService)
        {
            _vkClient = vkClient;
            _logger = logger;
            _vkMessageService = vkMessageService;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public async Task<ActionResult<DataSourceResponseModel>> GetData(int page, int pageSize,
            string search = null)
        {
            try
            {
                return await _vkMessageService.GetMessages(page, pageSize, search);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest();
            }
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpPost("repost")]
        public IActionResult Repost([FromBody]List<VkRepostViewModel> reposts, int timeout = 0)
        {
            try
            {
                _vkClient.Repost(reposts, timeout);
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