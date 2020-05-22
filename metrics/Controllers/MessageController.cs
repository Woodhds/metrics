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
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public MessageController(IMessageBroker messageBroker, ITransactionScopeFactory transactionScopeFactory)
        {
            _messageBroker = messageBroker;
            _transactionScopeFactory = transactionScopeFactory;
        }

        [HttpPost("type")]
        public async Task<IActionResult> SetType([FromBody] SetMessageTypeEvent @event)
        {
            await _messageBroker.PublishAsync(@event);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync([FromBody] MessageCategory category)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();
            if (category.Id > 0)
            {
                await scope.GetRepository<MessageCategory>().UpdateAsync(category);
            }
            else
            {
                await scope.GetRepository<MessageCategory>().CreateAsync(category);
            }

            await scope.CommitAsync();

            return Ok();
        }

        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<ActionResult<DataSourceResponseModel>> GetTypes(int page, int pageSize)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();
            var q = scope.GetRepository<MessageCategory>().Read().OrderBy(a => a.Id);
            return new DataSourceResponseModel(q.Skip(page * pageSize).Take(pageSize).ToList(), q.Count());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();
            await scope.GetRepository<MessageCategory>().DeleteAsync(new MessageCategory() {Id = id});
            await scope.CommitAsync();
            return Ok();
        }
    }
}