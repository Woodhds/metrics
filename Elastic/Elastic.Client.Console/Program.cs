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

            var page = 0;
            var take = 50;
            var search = "";

            var client = await factory.Create()
                .SearchAsync<VkMessageModel>(z => z
                    .From(page * take)
                    .Take(take)
                    .Highlight(s => s
                        .PreTags("<em>")
                        .PostTags("</em>")
                        .Fields(a => a
                            .Field(t => t.Text)
                        )
                    )
                    .Query(f => f
                        .Bool(e => e
                            .Filter(g => g
                                .MatchPhrase(n => n
                                    .Field(message => message.Text)
                                    .Query(search)
                                )
                            )
                        ))
                );

            System.Console.WriteLine(client.Documents.Count);
        }
    }
}