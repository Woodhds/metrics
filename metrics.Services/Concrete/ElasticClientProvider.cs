using System;
using metrics.Services.Models;
using metrics.Services.Options;
using Microsoft.Extensions.Options;
using Nest;

namespace Competition.Hosted
{
    public interface IElasticClientProvider
    {
        IElasticClient GetClient();
    }
    
    public class ElasticClientProvider : IElasticClientProvider
    {
        private readonly ElasticOptions _options;
        public ElasticClientProvider(IOptions<ElasticOptions> options)
        {
            _options = options.Value;
        }


        public IElasticClient GetClient()
        {
            var connection =
                new ConnectionSettings(new Uri(_options.Host))
                    .DefaultMappingFor<VkMessage>(descriptor =>
                    descriptor.IdProperty(model => model.Identifier).IndexName("vk_message"));
            return new ElasticClient(connection);
        }
    }
}