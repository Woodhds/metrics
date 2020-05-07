using System;
using metrics.ML.Contracts.Data;
using Metrics.Ml.Services;
using metrics.ML.Services.Abstractions;
using metrics.ML.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;

namespace metrics.ML.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictClient(this IServiceCollection serviceCollection, string serverAddress)
        {
            serviceCollection.AddSingleton<IMessagePredictModelService, MessagePredictModelService>();
            serviceCollection.AddGrpcClient<MessagePredicting.MessagePredictingClient>(options =>
            {
                options.Address = new Uri(serverAddress);
            });
            serviceCollection.AddPredictionEnginePool<VkMessageML, VkMessagePredict>()
                .FromFile("Model.zip", watchForChanges:true);
            
            return serviceCollection;
        }
    }
}