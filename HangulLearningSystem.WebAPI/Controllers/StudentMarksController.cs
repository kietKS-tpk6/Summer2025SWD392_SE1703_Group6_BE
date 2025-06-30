using Application.Usecases.Commands;
using Application.Usecases.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentMarksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentMarksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all scores of a test by test ID
        /// </summary>
        [HttpGet("test/{testId}/scores")]
        public async Task<IActionResult> GetTestScoresByTestId(string testId)
        {
            var query = new GetTestScoresByTestIdQuery { TestId = testId };
            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Update student marks from student test (system auto-grading)
        /// </summary>
        [HttpPost("update-from-student-test")]
        public async Task<IActionResult> UpdateFromStudentTest([FromBody] UpdateStudentMarksFromStudentTestCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Update student marks by lecturer (manual grading)
        /// </summary>
        [HttpPut("update-by-lecturer")]
        public async Task<IActionResult> UpdateByLecturer([FromBody] UpdateStudentMarksByLecturerCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Batch update student marks from multiple student tests (system auto-grading)
        /// </summary>
        [HttpPost("batch-update-from-student-tests")]
        public async Task<IActionResult> BatchUpdateFromStudentTests([FromBody] BatchUpdateStudentMarksFromStudentTestsCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}