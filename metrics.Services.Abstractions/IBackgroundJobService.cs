using System.Threading.Tasks;

namespace metrics.Services.Abstractions
{
    public interface IBackgroundJobService
    {
        Task RegisterRecurringJob();
        Task RegisterBackgroundJob();
    }
}