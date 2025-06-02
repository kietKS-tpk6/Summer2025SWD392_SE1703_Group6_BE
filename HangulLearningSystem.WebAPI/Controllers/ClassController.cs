using Application.Usecases.Command;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Common.Constants;
using System.Threading;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ClassCreateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.CreateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail);
            }
        }
    }
}
