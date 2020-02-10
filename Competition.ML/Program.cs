using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts.Options;
using Microsoft.ML;

namespace Competition.ML
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var context = new MLContext();
            var elasticClient = new ElasticClientFactory(new ElasticOptions
            {
                Host = "http://localhost:9200"
            });
        }
    }
}