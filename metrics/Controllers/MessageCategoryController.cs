using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.EventSourcing.Abstractions.Query;
using metrics.Queries;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageCategoryController : ControllerBase
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IQueryProcessor _queryProcessor;

        public MessageCategoryController(
            ITransactionScopeFactory transactionScopeFactory,
            IQueryProcessor queryProcessor
        )
        {
            _transactionScopeFactory = transactionScopeFactory;
            _queryProcessor = queryProcessor;
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
        public async Task<DataSourceResponseModel> GetTypes([FromRoute] MessageCategoryTypesQuery query)
        {
            return await _queryProcessor.ProcessAsync(query);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();
            await scope.GetRepository<MessageCategory>().DeleteAsync(new MessageCategory {Id = id});
            await scope.CommitAsync();
            return Ok();
        }
    }
}