using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Base.Contracts.Models;
using Elastic.Client;
using metrics.Competitions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

namespace metrics.Competitions.Hosted
{
    public interface ICompetitionService
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public class CompetitionService : ICompetitionService
    {
        private readonly IElasticClientFactory _elasticClientProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CompetitionService> _logger;

        public CompetitionService(
            IElasticClientFactory elasticClientProvider,
            IServiceProvider serviceProvider,
            ILogger<CompetitionService> logger
        )
        {
            _elasticClientProvider = elasticClientProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateUnbounded<VkMessageModel>();

            try
            {
                StartReading(channel.Reader, cancellationToken);
                await Task.WhenAll(_serviceProvider.GetServices<ICompetitionsService>()
                    .Select(f => f.Fetch(channel, cancellationToken: cancellationToken)));

                channel.Writer.Complete();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetch posts");
            }
            finally
            {
                Console.WriteLine($"Stop fetching \" {DateTimeOffset.Now}");
            }
        }

        async Task StartReading(ChannelReader<VkMessageModel> reader, CancellationToken cancellationToken = default)
        {
            var data = new List<VkMessageModel>();
            while (await reader.WaitToReadAsync(cancellationToken))
            {
                data.Add(await reader.ReadAsync(cancellationToken));
                if (data.Count <= Constants.MaximumIndexingBatchCount) 
                    continue;
                
                var result = await _elasticClientProvider.Create().IndexManyAsync(data, cancellationToken: cancellationToken);
                Console.WriteLine("ERROR OCCURED BY INDEXING: " + result.Errors);
                data.Clear();
            }

            if (data.Count > 0)
            {
                await _elasticClientProvider.Create().IndexManyAsync(data, cancellationToken: cancellationToken);
            }
        }
    }
}