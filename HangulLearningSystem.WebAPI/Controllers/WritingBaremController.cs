using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WritingBaremController : ControllerBase
    {
        private readonly IMediator _mediator;
       
        public WritingBaremController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("bulk-create")]
        public async Task<IActionResult> CreateWritingBarems([FromBody] CreateWritingBaremsCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
