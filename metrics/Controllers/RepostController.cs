using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using metrics.Services.Abstract;
using System.Linq;
using metrics.Services.Models;
using Microsoft.Extensions.Logging;
using metrics.Models;
using System.Net;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : Controller
    {
        private readonly IVkClient _vkClient;
        private readonly ILogger<RepostController> _logger;
        public RepostController(IVkClient vkClient, ILogger<RepostController> logger)
        {
            _vkClient = vkClient;
            _logger = logger;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public ActionResult<DataSourceResponseModel> GetData(string userId, int page, int pageSize, string search = null)
        {
            var data = _vkClient.GetReposts(userId, page, pageSize, search);
            var reposts = data.Response
                .Items.OrderByDescending(c => DateTimeOffset.FromUnixTimeSeconds(c.Date)).Distinct().ToList();
            return new DataSourceResponseModel(reposts, data.Response.Count);
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpPost("repost")]
        public IActionResult Repost([FromBody]List<VkRepostViewModel> reposts, int timeout = 0)
        {
            try
            {
                _vkClient.Repost(reposts, timeout);
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest();
            }
        }
    }
}