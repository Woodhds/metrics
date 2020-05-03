using System.Linq;
using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;

namespace metrics.Services.Concrete
{
    public class VkMessageService : IVkMessageService
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public VkMessageService(
            IElasticClientFactory elasticClientFactory,
            ITransactionScopeFactory transactionScopeFactory
        )
        {
            _elasticClientFactory = elasticClientFactory;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<DataSourceResponseModel> GetMessages(int page = 0, int take = 50, string search = null,
            string user = null)
        {
            var client = _elasticClientFactory.Create();

            var response = await client.SearchAsync<VkMessage>(z => z
                .From(page * take)
                .Take(take)
                .Query(f =>
                {
                    var q = f
                        .Bool(e => e
                            .Filter(g => g
                                .MatchPhrase(q => q
                                    .Field(message => message.Text)
                                    .Query(search)
                                )
                            )
                        );
                    if (string.IsNullOrEmpty(user)) return q;

                    return q &&
                           f
                               .Term(t => t
                                   .Value(user)
                                   .Field(l => l.RepostedFrom)
                               );
                })
            );
            using var scope = await _transactionScopeFactory.CreateAsync();
            var keys = response.Documents.Select(f => f.Owner_Id + "_" + f.Id);
            var items = scope.GetRepository<MessageVk>().Read().Select(r =>
                    new {Key = r.OwnerId.ToString() + "_" + r.MessageId.ToString(), item = r})
                .Where(e => keys.Contains(e.Key)).Select(f => f.item).ToList();

            foreach (var document in response.Documents)
            {
                document.MessageCategoryId = items.FirstOrDefault(f => f.MessageId == document.Id && f.OwnerId == document.Owner_Id)?.MessageCategoryId;
            }

            return new DataSourceResponseModel(response.Documents, response.Total);
        }
    }
}