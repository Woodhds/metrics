using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;

namespace metrics.Broker.Console
{
    public class RepostEventHandler : IMessageHandler<RepostEvent>
    {
        public Task HandleAsync(RepostEvent obj, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}