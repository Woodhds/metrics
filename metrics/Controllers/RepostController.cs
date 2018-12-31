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
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : ControllerBase
    {
        private readonly IVkClient _vkClient;
        private readonly ILogger<RepostController> _logger;
        private readonly DbContext _dataContext;
        public RepostController(IVkClient vkClient, ILogger<RepostController> logger, DbContext dataContext)
        {
            _vkClient = vkClient;
            _logger = logger;
            _dataContext = dataContext;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public async Task<ActionResult<DataSourceResponseModel>> GetData(string userId, int page, int pageSize,
            string search = null, bool fromRepo = false)
        {
            if (!fromRepo)
            {
                var data = _vkClient.GetReposts(userId.Trim(), page, pageSize, search);
                if(data.Response.Items == null)
                    data.Response.Items = new List<VkMessage>();
                return new DataSourceResponseModel(data.Response.Items, data.Response.Count);
            }

            var messages = _dataContext.Set<ParseMessage>().Where(c => c.Text.Contains(search));
            var count = await messages.CountAsync();
            var vkresponse = await messages.OrderByDescending(c => c.Date).Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            var response =
                _vkClient.GetById(vkresponse.Select(c => new VkRepostViewModel {Id = c.Id, Owner_Id = c.OwnerId}));
            
            return new DataSourceResponseModel(response.Response.Items, count);
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
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(false);
            }
        }
    }
}