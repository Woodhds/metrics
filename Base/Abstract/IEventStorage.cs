using System.Threading.Tasks;

namespace Base.Abstract
{
    public interface IEventStorage
    {
        Task AddEvent(IRequest request);
    }
}