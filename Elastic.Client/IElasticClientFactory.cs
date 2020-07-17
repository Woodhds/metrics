using Nest;

namespace Elastic.Client
{
    public interface IElasticClientFactory
    {
        IElasticClient Create();
    }
}