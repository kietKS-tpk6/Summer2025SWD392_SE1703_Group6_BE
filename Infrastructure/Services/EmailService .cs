using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Domain.Entities;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService, IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<EmailService> _logger;
        private readonly IOTPRepository _OTPRepository;
        private const int TIME_TO_USE_OTP_MINUTES = 5;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IOTPRepository oTPRepository)
        {
            _logger = logger;
            _OTPRepository = oTPRepository;
            var emailSettings = configuration.GetSection("EmailSettings");

            _fromEmail = emailSettings["FromEmail"];
            _fromName = emailSettings["FromName"] ?? "HangulLearning System";

            _smtpClient = new SmtpClient(emailSettings["SmtpHost"])
            {
                Port = int.Parse(emailSettings["SmtpPort"]),
                Credentials = new NetworkCredential(_fromEmail, emailSettings["Password"]),
                EnableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true"),
                Timeout = 30000 // 30 seconds timeout
            };

        }

        private string GenerateOtpCode(int length = 6)
        {
            if (length < 4 || length > 8)
                throw new ArgumentException("OTP length must be between 4 and 8 digits");

            Random random = new Random();
            string otp = "";

            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();
            }

            return otp;
        }
        public async Task<bool> SendOtpEmailAsync(string toEmail)
        {
            try
            {
                string otpCode = GenerateOtpCode();
                var subject = "Mã xác thực OTP";
                string emailBody = CreateOtpEmailTemplate(otpCode);
               var otp = new OTP();
                otp.ExpirationTime = DateTime.UtcNow;
                otp.OTPCode = otpCode;
                otp.PhoneNumber = null;
                otp.Email = toEmail;
                otp.ExpirationTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow.AddMinutes(TIME_TO_USE_OTP_MINUTES),
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                );
                var res = await _OTPRepository.createOTP(otp);
                if (res) return await SendEmailAsync(toEmail, subject, emailBody, true);
               return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mail.To.Add(toEmail);

                await _smtpClient.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email to {Email}: {Message}", toEmail, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                string subject = "Chào mừng bạn đến với HangulLearning System!";
                string emailBody = CreateWelcomeEmailTemplate(userName);
                return await SendEmailAsync(toEmail, subject, emailBody, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            try
            {
                string subject = "Yêu cầu đặt lại mật khẩu";
                string emailBody = CreatePasswordResetEmailTemplate(resetLink);
                return await SendEmailAsync(toEmail, subject, emailBody, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
                return false;
            }
        }

        #region Email Templates

        private string CreateOtpEmailTemplate(string otpCode)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Mã xác thực OTP</title>
            </head>
            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 0;'>
                    <!-- Header -->
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 28px; font-weight: bold;'>HangulLearning</h1>
                        <p style='color: #e8e8e8; margin: 10px 0 0 0; font-size: 16px;'>Hệ thống học tiếng Hàn</p>
                    </div>
                    
                    <!-- Content -->
                    <div style='padding: 40px 30px;'>
                        <h2 style='color: #333; text-align: center; margin: 0 0 20px 0; font-size: 24px;'>Mã xác thực OTP</h2>
                        <p style='color: #666; font-size: 16px; line-height: 1.5; text-align: center; margin: 0 0 30px 0;'>
                            Chúng tôi đã nhận được yêu cầu xác thực tài khoản của bạn. Vui lòng sử dụng mã OTP bên dưới:
                        </p>
                        
                        <!-- OTP Box -->
                        <div style='background-color: #f8f9ff; border: 2px dashed #667eea; border-radius: 12px; padding: 30px; text-align: center; margin: 30px 0;'>
                            <p style='color: #666; font-size: 14px; margin: 0 0 10px 0; text-transform: uppercase; letter-spacing: 1px;'>MÃ XÁC THỰC</p>
                            <div style='font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; font-family: ""Courier New"", monospace;'>{otpCode}</div>
                        </div>
                        
                        <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 20px 0;'>
                            <p style='color: #856404; margin: 0; font-size: 14px; text-align: center;'>
                                ⚠️ <strong>Lưu ý:</strong> Mã này sẽ hết hạn sau <strong>5 phút</strong> và chỉ được sử dụng một lần
                            </p>
                        </div>
                        
                        <p style='color: #666; font-size: 14px; line-height: 1.5; text-align: center; margin: 20px 0 0 0;'>
                            Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi.
                        </p>
                    </div>
                    
                    <!-- Footer -->
                    <div style='background-color: #f8f9fa; padding: 20px 30px; text-align: center; border-top: 1px solid #e9ecef;'>
                        <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                            © 2025 HangulLearning System. All rights reserved.
                        </p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private string CreateWelcomeEmailTemplate(string userName)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Chào mừng đến với HangulLearning</title>
            </head>
            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: white;'>
                    <!-- Header -->
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Chào mừng đến với HangulLearning!</h1>
                    </div>
                    
                    <!-- Content -->
                    <div style='padding: 40px 30px;'>
                        <h2 style='color: #333; margin: 0 0 20px 0;'>Xin chào {userName}!</h2>
                        <p style='color: #666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                            Cảm ơn bạn đã đăng ký tài khoản tại HangulLearning System. Chúng tôi rất vui mừng chào đón bạn!
                        </p>
                        
                        <div style='background-color: #f8f9ff; border-left: 4px solid #667eea; padding: 20px; margin: 20px 0;'>
                            <h3 style='color: #667eea; margin: 0 0 10px 0;'>🚀 Bắt đầu hành trình học tiếng Hàn:</h3>
                            <ul style='color: #666; margin: 0; padding-left: 20px;'>
                                <li>Học bảng chữ cái Hangul cơ bản</li>
                                <li>Luyện tập từ vựng hàng ngày</li>
                                <li>Thực hành ngữ pháp với bài tập</li>
                                <li>Theo dõi tiến độ học tập</li>
                            </ul>
                        </div>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='#' style='background-color: #667eea; color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>Bắt đầu học ngay</a>
                        </div>
                        
                        <p style='color: #666; font-size: 14px;'>
                            Nếu bạn có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi!
                        </p>
                    </div>
                    
                    <!-- Footer -->
                    <div style='background-color: #f8f9fa; padding: 20px 30px; text-align: center;'>
                        <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                            © 2025 HangulLearning System. All rights reserved.
                        </p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private string CreatePasswordResetEmailTemplate(string resetLink)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Đặt lại mật khẩu</title>
            </head>
            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: white;'>
                    <!-- Header -->
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 28px;'>🔒 Đặt lại mật khẩu</h1>
                        <p style='color: #e8e8e8; margin: 10px 0 0 0;'>HangulLearning System</p>
                    </div>
                    
                    <!-- Content -->
                    <div style='padding: 40px 30px;'>
                        <h2 style='color: #333; margin: 0 0 20px 0;'>Yêu cầu đặt lại mật khẩu</h2>
                        <p style='color: #666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                            Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.
                        </p>
                        
                        <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 20px 0;'>
                            <p style='color: #856404; margin: 0; font-size: 14px;'>
                                ⚠️ <strong>Lưu ý:</strong> Link này sẽ hết hạn sau <strong>15 phút</strong>
                            </p>
                        </div>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>Đặt lại mật khẩu</a>
                        </div>
                        
                        <p style='color: #666; font-size: 14px; line-height: 1.5;'>
                            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này. 
                            Mật khẩu của bạn sẽ không thay đổi.
                        </p>
                        
                        <p style='color: #666; font-size: 12px; margin-top: 20px; padding: 15px; background-color: #f8f9fa; border-radius: 5px;'>
                            <strong>Không thể click vào nút?</strong><br>
                            Copy link sau vào trình duyệt: <br>
                            <span style='word-break: break-all; color: #667eea;'>{resetLink}</span>
                        </p>
                    </div>
                    
                    <!-- Footer -->
                    <div style='background-color: #f8f9fa; padding: 20px 30px; text-align: center;'>
                        <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                            © 2025 HangulLearning System. All rights reserved.
                        </p>
                    </div>
                </div>
            </body>
            </html>";
        }

        #endregion

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
