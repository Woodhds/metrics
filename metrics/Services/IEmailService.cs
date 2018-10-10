using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace metrics.Services
{
    public interface IEmailService
    {
        Task SendAsync(string title, string body, List<string> to);
    }
}
