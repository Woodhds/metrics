using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker
{
    public class MessageBrokerHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MessageBrokerHostedService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Bus started: " + DateTimeOffset.Now);
            return _busControl.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
            Console.WriteLine("Bus stopped: " + DateTimeOffset.Now);
        }
    }
}