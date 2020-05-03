using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Data.Abstractions;
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

            return new DataSourceResponseModel(response.Documents, response.Total);
        }
    }
}