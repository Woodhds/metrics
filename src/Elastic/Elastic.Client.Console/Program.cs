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

            await GetDuplicates(factory.Create());
        }

        static async Task GetDuplicates(IElasticClient client)
        {
            var result = await client.SearchAsync<VkMessageModel>(descriptor => descriptor.MatchAll().Skip(0).Take(10000));
            var grouped = result.Documents
                .GroupBy(f => new {f.OwnerId, f.Id});
            
            var duplicated = grouped
                .Where(f => f.Count() > 1)
                .Select(f => f.FirstOrDefault(a => a.FromId == 0))
                .ToList();
            
            var resultDeleteByQueryAsync = await client.DeleteByQueryAsync<VkMessageModel>(f => f
                .Query(t => t
                    .Ids(q => q
                        .Values(duplicated.Select(f => f.Identifier.ToString()))
                    )
                )
            );

            await System.Console.Out.WriteLineAsync("EXECUTION DEBUG: " + resultDeleteByQueryAsync.DebugInformation);
            await System.Console.Out.WriteLineAsync("EXECUTION RESULT: " + resultDeleteByQueryAsync.Deleted);
        }
    }
}