using metrics.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace metrics.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailOptions _mailOptions;
        public EmailService(IOptions<MailOptions> options)
        {
            _mailOptions = options.Value;
        }

        public async Task SendAsync(string title, string body, List<string> to)
        {
            using(var smtpClient = new SmtpClient(_mailOptions.Host, _mailOptions.Port))
            {
                var message = new MailMessage();
                message.Body = body;
                message.Subject = title;
                to.ForEach(x => message.To.Add(x));
                message.IsBodyHtml = true;
                message.From = new MailAddress(_mailOptions.From);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = _mailOptions.UseSSL;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(_mailOptions.UserName, _mailOptions.Password);
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
