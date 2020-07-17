using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Options;
using Elastic.Client;
using Microsoft.Extensions.Options;
using Nest;

namespace Elastic.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = Options.Create(new ElasticOptions
            {
                Host = "http://localhost:9200"
            });

           var client = new ElasticClientFactory(options)
                .Create();
           
           if (!(await client.Indices.ExistsAsync(Indices.Index("vk_message"))).Exists)
           {
               await client.Indices.CreateAsync(IndexName.From<VkMessage>("vk_message"));
           }
           if (!(await client.Indices.ExistsAsync(Indices.Index("vk_user"))).Exists)
           {
               await client.Indices.CreateAsync(IndexName.From<VkUserModel>("vk_user"));
           }
        }
    }
}