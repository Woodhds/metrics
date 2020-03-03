using System.Threading.Tasks;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Competition.ML
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddVkClientConsole();
            services.AddSingleton<IVkClient, VkClient>();
        }
    }
}