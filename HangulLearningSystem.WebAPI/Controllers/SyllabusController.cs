using Application.Usecases.Command;
using Application.Common.Constants;

using Application.Usecases.CommandHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SyllabusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("create-syllabus")]
        public async Task<IActionResult> CreateSyllabus([FromBody] CreateSyllabusesCommand command, CancellationToken cancellationToken)
        {
            var accountId = User.FindFirst("AccountID")?.Value;

            if (accountId == null)
                return Unauthorized("Không tìm thấy accountID trong token.");

            command.AccountID = accountId; 

            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                return BadRequest("Tạo chương trình học thất bại");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("update-syllabus")]
        public async Task<IActionResult> UpdateSyllabus([FromBody] UpdateSyllabusesCommand command, CancellationToken cancellationToken)
        {
            var accountId = User.FindFirst("AccountID")?.Value;

            if (accountId == null)
                return Unauthorized("Không tìm thấy accountID trong token.");

            command.AccountID = accountId;

            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                return BadRequest("Tạo chương trình học thất bại");
            }
            return Ok(result);
        }
    }
}
