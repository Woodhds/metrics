using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageCategoryController : ControllerBase
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public MessageCategoryController(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
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
        public ActionResult<DataSourceResponseModel> GetTypes(int page, int pageSize)
        {
            using var scope = _transactionScopeFactory.CreateQuery();
            var q = scope.Query<MessageCategory>().OrderBy(a => a.Id);
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