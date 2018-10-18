using metrics.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace metrics.Services.Abstract
{
    public interface IVkClient : IBaseHttpClient
    {
        Task<VkResponse<List<VkMessage>>> GetReposts(string id, int skip, int take);
    }
}
