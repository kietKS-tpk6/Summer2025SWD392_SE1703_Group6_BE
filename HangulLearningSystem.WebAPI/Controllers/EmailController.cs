using Application.IServices;
using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    
    {
        private readonly SendOTPViaEmailCommandHandler _sendOTPViaEmailCommandHandler;
        private readonly IEmailService _emailService;

        public EmailController(SendOTPViaEmailCommandHandler sendOTPViaEmailCommandHandler, IEmailService emailService)
        {
            _sendOTPViaEmailCommandHandler = sendOTPViaEmailCommandHandler;
            _emailService = emailService;
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
    }
}
