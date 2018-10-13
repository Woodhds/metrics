using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace metrics.Services.Abstract
{
    public interface IUserManagerService
    {
        Task SendEmailConfirmation(User user);
    }
}
