using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using metrics.Services.Abstract;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using metrics.Services.Models;
using Microsoft.Extensions.Logging;
using metrics.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : ControllerBase
    {
        private readonly IVkClient _vkClient;
        private readonly ILogger<RepostController> _logger;
        private readonly ICompetitionsService _competitionsService;

        public RepostController(IVkClient vkClient, ILogger<RepostController> logger,
            ICompetitionsService competitionsService)
        {
            _vkClient = vkClient;
            _logger = logger;
            _competitionsService = competitionsService;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public async Task<ActionResult<DataSourceResponseModel>> GetData(string userId, int page, int pageSize,
            string search = null)
        {
            try
            {
                var data = _vkClient.GetReposts(userId.Trim(), page, pageSize, search);
                if (data.Response.Items == null)
                    data.Response.Items = new List<VkMessage>();
                return new DataSourceResponseModel(data.Response.Items, data.Response.Count);
            }
            catch (Exception e)
            {
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

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("site")]
        public async Task<ActionResult<DataSourceResponseModel>> GetFromSite()
        {
            try
            {
                var data = await _competitionsService.Fetch();
                return new DataSourceResponseModel(data, data.Count);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}