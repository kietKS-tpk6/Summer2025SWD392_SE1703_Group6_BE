using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
