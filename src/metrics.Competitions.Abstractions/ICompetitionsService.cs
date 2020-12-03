using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Base.Contracts.Models;

namespace metrics.Competitions.Abstractions
{
    public interface ICompetitionsService
    {
        Task Fetch(ChannelWriter<VkMessageModel> writer, int page = 1, CancellationToken cancellationToken = default);
    }
}