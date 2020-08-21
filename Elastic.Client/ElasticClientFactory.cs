using System;
using Base.Contracts;
using Base.Contracts.Options;
using Microsoft.Extensions.Options;
using Nest;

namespace Elastic.Client
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ElasticOptions _options;

        public ElasticClientFactory(IOptions<ElasticOptions> options)
        {
            _options = options.Value;
        }

        public ElasticClientFactory(ElasticOptions options)
        {
            _options = options;
        }

        public IElasticClient Create()
        {
            var connection =
                new ConnectionSettings(new Uri(_options.Host))
                    .DisableDirectStreaming()
                    .DefaultMappingFor<VkMessage>(descriptor =>
                        descriptor
                            .IdProperty(model => model.Identifier)
                            .IndexName("vk_message")
                    )
                    .DefaultMappingFor<VkUserModel>(descriptor => descriptor.IdProperty(model => model.Id)
                        .IndexName("vk_user"));
            return new ElasticClient(connection);
        }
    }
}