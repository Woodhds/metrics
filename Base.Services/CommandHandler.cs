using System.Threading.Tasks;
using Base.Abstract;

namespace Base.Services
{
    public class CommandHandler<T>: ICommandHandler<T> where T: IRequest
    {
        private readonly IEventStorage _eventStorage;

        public CommandHandler(IEventStorage eventStorage)
        {
            _eventStorage = eventStorage;
        }

        public async Task Execute(T command)
        {
            await _eventStorage.AddEvent(command);
        }
    }
}