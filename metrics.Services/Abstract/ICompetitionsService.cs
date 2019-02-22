using System.Collections.Generic;
using System.Threading.Tasks;
using metrics.Services.Models;

namespace metrics.Services.Abstract
{
    public interface ICompetitionsService
    {
        Task<List<VkMessage>> Fetch();
    }
}