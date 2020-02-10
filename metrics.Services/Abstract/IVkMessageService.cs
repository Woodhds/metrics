using System.Threading.Tasks;
using Base.Contracts;

namespace metrics.Services.Abstract
{
    public interface IVkMessageService
    {
        Task<DataSourceResponseModel> GetMessages(int page = 0, int take = 50, string search = null);
    }
}