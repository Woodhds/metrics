using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts.Models;

namespace metrics.Competitions.Abstractions
{
    public interface ICompetitionsService
    {
        Task<IList<VkMessageModel>> Fetch(int page = 1);
    }
}