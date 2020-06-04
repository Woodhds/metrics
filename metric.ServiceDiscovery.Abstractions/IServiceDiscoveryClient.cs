using System.Threading.Tasks;

namespace metric.ServiceDiscovery.Abstractions
{
    public interface IServiceDiscoveryClient
    {
        Task RegisterService();
        Task DeregisterService();
    }
}