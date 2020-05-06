using System;
using Metrics.Ml.Services;
using metrics.ML.Services.Abstractions;
using metrics.ML.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.ML.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictClient(this IServiceCollection serviceCollection, string serverAddress)
        {
            serviceCollection.AddScoped<IMessagePredictModelService, MessagePredictModelService>();
            serviceCollection.AddGrpcClient<MessagePredicting.MessagePredictingClient>(options =>
            {
                options.Address = new Uri(serverAddress);
            });
            return serviceCollection;
        }
    }
}