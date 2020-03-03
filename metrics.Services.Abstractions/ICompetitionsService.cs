using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions
{
    public interface ICompetitionsService
    {
        Task<List<VkMessage>> Fetch(int page = 1);
    }
}