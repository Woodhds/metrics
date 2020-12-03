using System.Threading;
using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions
{
    public interface ICommandHandler<in TCommand, TResult> where TCommand: ICommand
    {
        Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }
}