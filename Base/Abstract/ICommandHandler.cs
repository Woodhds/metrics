using System.Threading.Tasks;

namespace Base.Abstract
{
    public interface ICommandHandler<in T>
    {
        Task Execute(T command);
    }
}