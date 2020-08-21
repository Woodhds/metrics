using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Competitions.Abstractions
{
    public interface ICompetitionsService
    {
        Task<IList<VkMessage>> Fetch(int page = 1);
    }
}