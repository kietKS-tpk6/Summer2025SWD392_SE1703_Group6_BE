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
        private readonly RegisterCommandHandler _registerCommandHandler;

        public AuthenticationController(LoginCommandHandler loginCommandHandler, RegisterCommandHandler registerCommandHandler)
        {
            _loginCommandHandler = loginCommandHandler;
            _registerCommandHandler = registerCommandHandler;
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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand registerCommand)
        {
            var result = await _registerCommandHandler.Handle(registerCommand, CancellationToken.None);
            if (result == null)
            {
                return BadRequest("");
            }
            return Ok(result);
        }
    }
}
