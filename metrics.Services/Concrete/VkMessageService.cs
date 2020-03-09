using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using metrics.Services.Abstractions;

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
                .Highlight(f => f
                    .Fields(j => j
                        .Field(y => y.Text)
                        .PreTags("<em>")
                        .PostTags("</em>")
                    )
                )
                .Take(take)
                .Query(f => f
                    .Bool(e => e
                        .Filter(g => g
                            .MatchPhrase(q => q
                                .Field(message => message.Text)
                                .Query(search)
                            )
                        )
                        .Should(t => t
                            .Term(h => h
                                .Field(m => m.RepostedFrom)
                                .Value(user)
                            )
                        )
                    )
                )
            );

            return new DataSourceResponseModel(response.Documents, response.Total);
        }
    }
}