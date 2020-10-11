using System.Linq;
using System.Threading.Tasks;
using Base.Contracts.Models;
using Base.Contracts.Options;
using Nest;

namespace Elastic.Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ElasticClientFactory(new ElasticOptions
            {
                Host = "http://localhost:9200"
            });
        }

        async Task GetDuplicates(IElasticClient client)
        {
            var result = await client.SearchAsync<VkMessageModel>(descriptor => descriptor.MatchAll());
            var duplicated = result.Documents.GroupBy(f => new {f.OwnerId, f.Id}).Select(f => f.FirstOrDefault())
                .ToList();
        }

        async Task WriteLog(IElasticClient client)
        {
            
        }
    }
}