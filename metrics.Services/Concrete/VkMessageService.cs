using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Services.Abstractions;
using Nest;

namespace metrics.Services.Concrete
{
    public class VkMessageService : IVkMessageService
    {
        private readonly IElasticClientFactory _elasticClientFactory;

        public VkMessageService(IElasticClientFactory elasticClientFactory)
        {
            _elasticClientFactory = elasticClientFactory;
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