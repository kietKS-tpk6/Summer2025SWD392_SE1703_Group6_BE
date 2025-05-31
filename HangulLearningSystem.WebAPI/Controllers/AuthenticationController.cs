using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly LoginCommandHandler _loginCommandHandler;
        public AuthenticationController(LoginCommandHandler loginCommandHandler)
        {
            _loginCommandHandler = loginCommandHandler;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
         var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);
            if (result == null)
            {
                return BadRequest("");
            }
            return Ok(result);
        }
    }
}
