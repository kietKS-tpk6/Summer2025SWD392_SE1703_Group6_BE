using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SyllabusScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-syllabus-schedule")]
        public async Task<IActionResult> CreateAccountByManager([FromBody] SyllabusScheduleCreateCommand command, CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
