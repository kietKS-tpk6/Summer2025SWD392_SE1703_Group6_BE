using Application.IServices;
using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    
    {
        private readonly SendOTPViaEmailCommandHandler _sendOTPViaEmailCommandHandler;
        private readonly IEmailService _emailService;
        private readonly IClassService _classService;
        private readonly ICertificateService _certificateService;

        private readonly IMediator _mediator;

        public EmailController(SendOTPViaEmailCommandHandler sendOTPViaEmailCommandHandler, IEmailService emailService, IMediator mediator, ICertificateService certificateService)
        {
            _sendOTPViaEmailCommandHandler = sendOTPViaEmailCommandHandler;
            _emailService = emailService;
            _mediator = mediator;
            _certificateService = certificateService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOTPViaEmailCommand command)
        {
            var result = await _sendOTPViaEmailCommandHandler.Handle(command, CancellationToken.None);

            if (result)
            {
                return Ok("gửi OTP thành công");
            }

            return BadRequest("gửi OTP thất bại");
        }
        [HttpPost("classes/notify-students-start/{classId}")]
        public async Task<IActionResult> NotifyStudentsStartClass(string classId)
        {
            var result = await _emailService.SendClassStartNotificationAsync(classId);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("classes/notify-lesson-update/{classId}")]
        public async Task<IActionResult> NotifyStudentsLessonUpdate(string classId)
        {
            var result = await _emailService.SendLessonUpdateNotificationAsync(classId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("classes/notify-students-cancelled/{classId}")]
        public async Task<IActionResult> NotifyStudentsCancelledClass(string classId)
        {
            var result = await _emailService.SendClassCancelledEmailAsync(classId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPost("send-certificate")]
        public async Task<IActionResult> SendCertificateAsync([FromBody] SendCertificateToStudentCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success)
                return Ok("Đã gửi chứng chỉ thành công.");
            return BadRequest(result.Message);
        }
       
        [HttpPost("send-certificate/class/{classId}")]
        public async Task<IActionResult> SendCertificateToClass(string classId)
        {
            var result = await _certificateService.SendCertificatesToClassAsync(classId);

            if (!result.Success)
            {
                // Trường hợp lỗi hệ thống hoặc không tìm thấy lớp/học sinh
                return BadRequest(new
                {
                    result.Message,
                    Errors = result.Data // Danh sách học sinh thất bại (nếu có)
                });
            }

            if (result.Data != null && result.Data.Any())
            {
                // Một số học sinh lỗi khi gửi
                return Ok(new
                {
                    Message = "Gửi chứng chỉ cho lớp hoàn tất, nhưng có lỗi với một số học sinh.",
                    FailedStudents = result.Data
                });
            }

            // Tất cả học sinh gửi thành công
            return Ok(new
            {
                Message = "Đã gửi chứng chỉ cho tất cả học sinh trong lớp thành công."
            });
        }


    }
}
