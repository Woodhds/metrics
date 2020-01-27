using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstract
{
    public interface ICompetitionsService
    {
        Task<List<VkMessage>> Fetch(int page = 1);
    }
}