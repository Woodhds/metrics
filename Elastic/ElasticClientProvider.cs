using System;
using Base.Contracts;
using Base.Contracts.Options;
using Microsoft.Extensions.Options;
using Nest;

namespace Base.Abstractions
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