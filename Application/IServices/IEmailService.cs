using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendWelcomeEmailWithPassAsync(string toEmail, string userName,string pass);
        
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task<OperationResult<bool>> SendClassStartNotificationAsync(string classId);
        Task<OperationResult<bool>> SendLessonUpdateNotificationAsync(string classId);
        Task<OperationResult<bool>> SendClassCancelledEmailAsync(string classId);
    }
}
