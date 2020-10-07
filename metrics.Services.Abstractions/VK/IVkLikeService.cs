using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstractions.VK
{
    public interface IVkLikeService
    {
        Task<SimpleVkResponse<VkResponseLikeModel>> Like(VkRepostViewModel model);
    }
}