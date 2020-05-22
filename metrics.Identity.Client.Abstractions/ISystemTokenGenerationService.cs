using System.Threading.Tasks;

namespace metrics.Identity.Client.Abstractions
{
    public interface ISystemTokenGenerationService
    {
        string GetSystemToken();
    }
}