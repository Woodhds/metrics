using Nest;

namespace Base.Abstractions
{
    public interface IElasticClientFactory
    {
        IElasticClient Create();
    }
}