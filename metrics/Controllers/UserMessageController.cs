using System.Threading.Tasks;
using Base.Contracts;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserMessageController : ControllerBase
    {
        private readonly IQueryProcessor _queryProcessor;
        public UserMessageController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        [HttpGet]
        public async Task<DataSourceResponseModel> GetUserMessages([FromQuery]UserMessageQuery query)
        {
            return await _queryProcessor.ProcessAsync(query);
        }
    }
}