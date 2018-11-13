using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using metrics.Services.Abstract;
using System.Linq;
using System.Threading.Tasks;
using metrics.Services.Models;
using Microsoft.Extensions.Logging;
using metrics.Models;
using metrics.Services.Options;
using Microsoft.Extensions.Options;
using Nest;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepostController : Controller
    {
        private readonly IVkClient _vkClient;
        private readonly ILogger<RepostController> _logger;
        private readonly CompetitionOptions _options;
        public RepostController(IVkClient vkClient, ILogger<RepostController> logger, IOptions<CompetitionOptions> options)
        {
            _options = options.Value;
            _vkClient = vkClient;
            _logger = logger;
        }

        [Authorize(Policy = "VkPolicy")]
        [HttpGet("user")]
        public async Task<ActionResult<DataSourceResponseModel>> GetData(string userId, int page, int pageSize, string search = null)
        {
            var connectionSettings = new ConnectionSettings(new Uri(_options.Address)).DisableDirectStreaming();
            var es = new ElasticClient(connectionSettings);
            var d = await es.SearchAsync<VkMessage>(z => z.Index(_options.Index).Query(t =>
                t.Bool(g => g.Must(l => l.MatchPhrase(e => e.Field(h => h.Text).Query("13 ноября"))))));
            
            var data = _vkClient.GetReposts(userId, page, pageSize, search);
            var reposts = data.Response
                .Items.Where(c => c.Reposts != null && !c.Reposts.User_reposted)
                .OrderByDescending(c => DateTimeOffset.FromUnixTimeSeconds(c.Date)).Distinct().ToList();
            return new DataSourceResponseModel(reposts, data.Response.Count);
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