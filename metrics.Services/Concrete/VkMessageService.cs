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

            var query = new BoolQuery
            {
                Filter = new QueryContainer[]
                {
                    new MatchPhraseQuery
                    {
                        Field = nameof(VkMessage.Text),
                        Query = search
                    }
                },
                Must = !string.IsNullOrEmpty(user)
                    ? new QueryContainer[]
                    {
                        new TermQuery
                        {
                            Field = nameof(VkMessage.RepostedFrom), 
                            Value = user
                        }
                    }
                    : new QueryContainer[0]
            };

            var searchQuery = new SearchRequest(Indices.Index<VkMessage>())
            {
                From = page * take,
                Query = query,
                Size = take
            };

            var response = await client.SearchAsync<VkMessage>(searchQuery);

            return new DataSourceResponseModel(response.Documents, response.Total);
        }
    }
}