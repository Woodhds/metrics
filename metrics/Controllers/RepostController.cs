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
    public class RepostController : ControllerBase
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
        public async Task<ActionResult<DataSourceResponseModel>> GetData(string userId, int page, int pageSize,
            string search = null, bool fromRepo = false)
        {
            if (!fromRepo)
            {
                var data = _vkClient.GetReposts(userId, page, pageSize, search);
                if(data.Response.Items == null)
                    data.Response.Items = new List<VkMessage>();
                return new DataSourceResponseModel(data.Response.Items, data.Response.Count);
            }

            var connectionSettings = new ConnectionSettings(new Uri(_options.Address)).DisableDirectStreaming();
            var es = new ElasticClient(connectionSettings);
            var messages = await es.SearchAsync<VkMessage>(descriptor =>
                descriptor.Index(_options.Index).Query(z =>
                        z.Bool(r => r.Must(q =>
                            q.MatchPhrase(queryDescriptor => queryDescriptor.Field(e => e.Text).Query(search)))))
                    .Sort(z => z.Field(fieldDescriptor => fieldDescriptor.Descending().Field(e => e.Date)))
                    .Take(pageSize).Skip((page - 1) * pageSize));
            var response = _vkClient.GetById(messages.Documents.Distinct()
                .Select(c => new VkRepostViewModel {Id = c.Id, Owner_Id = c.Owner_Id}));
            return new DataSourceResponseModel(response.Response.Items, messages.Total);
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