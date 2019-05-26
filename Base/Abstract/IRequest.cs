using System.Threading.Tasks;

namespace Base.Abstract
{
    public interface IRequest
    {
        CommandType Type { get; }
        int Timeout { get; }
        Task Handle();
    }
}