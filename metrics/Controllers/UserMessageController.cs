using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Authentication.Infrastructure;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace metrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserMessageController : ControllerBase
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
        private readonly IVkClient _vkClient;

        public UserMessageController(ITransactionScopeFactory transactionScopeFactory,
            IAuthenticatedUserProvider authenticatedUserProvider, IVkClient vkClient)
        {
            _transactionScopeFactory = transactionScopeFactory;
            _authenticatedUserProvider = authenticatedUserProvider;
            _vkClient = vkClient;
        }

        [HttpGet]
        public async Task<DataSourceResponseModel> GetUserMessages(int page = 0, int pageSize = 50)
        {
            using var scope = await _transactionScopeFactory.CreateAsync();

            var query = scope.Query<VkRepost>()
                .Where(f => f.UserId == _authenticatedUserProvider.GetUser().Id).OrderByDescending(f => f.Id);

            var data = await query.Skip(page * pageSize).Take(pageSize).ToListAsync();
            var texts = (await _vkClient.GetById(
                    data.Select(f => new VkRepostViewModel(f.OwnerId, f.MessageId))))
                .Response.Items
                .Select(f => new {f.Text});

            return new DataSourceResponseModel(texts, await query.CountAsync());
        }
    }
}