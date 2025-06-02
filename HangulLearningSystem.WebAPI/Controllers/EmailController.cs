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

        public EmailController(SendOTPViaEmailCommandHandler sendOTPViaEmailCommandHandler)
        {
            _sendOTPViaEmailCommandHandler = sendOTPViaEmailCommandHandler;
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
    }
}
