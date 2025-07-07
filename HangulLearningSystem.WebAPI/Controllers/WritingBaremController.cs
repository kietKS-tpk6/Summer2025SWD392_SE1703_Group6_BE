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
        [HttpPut("Update-barem")]
        public async Task<IActionResult> UpdateWritingBarem( [FromBody] UpdateWritingBaremCommand command)
        {
          
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{questionID}")]
        public async Task<IActionResult> GetWritingBaremsByQuestionID(string questionID)
        {
            var command = new GetWritingBaremsByQuestionIDCommand { QuestionID = questionID };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("by-test/{testID}")]
        public async Task<IActionResult> GetWritingQuestionsByTestID(string testID)
        {
            var command = new GetWritingQuestionsByTestIDCommand { TestID = testID };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{writingBaremID}")]
        public async Task<IActionResult> DeleteWritingBarem(string writingBaremID)
        {
            var command = new DeleteWritingBaremCommand { WritingBaremID = writingBaremID };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
