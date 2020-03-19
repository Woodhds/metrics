using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;

namespace metrics.Broker.Console
{
    public class GroupJoinEventHandler : IMessageHandler<GroupJoinEvent>
    {
        public Task HandleAsync(GroupJoinEvent obj, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }
    }
}