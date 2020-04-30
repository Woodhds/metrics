using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Broker.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Events;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public MessageController(IMessageBroker messageBroker, ITransactionScopeFactory transactionScopeFactory)
        {
            _messageBroker = messageBroker;
            _transactionScopeFactory = transactionScopeFactory;
        }

        [HttpPost]
        public async Task<IActionResult> SetType([FromBody]SetMessageTypeEvent @event)
        {
            await _messageBroker.PublishAsync(@event);
            return Ok();
        }

        [HttpGet("types/{page:int}/{pageSize:int}")]
        public async Task<ActionResult<DataSourceResponseModel>> GetTypes(int page, int pageSize)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();
            var q = scope.GetRepository<MessageCategory>().Read().OrderBy(a => a.Id);
            return new DataSourceResponseModel(q.Skip(page * pageSize).Take(pageSize).ToList(), q.Count());
        }
    }
}