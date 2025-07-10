using Application.IServices;
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
    //[Authorize]
    public class StudentMarksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStudentMarksService _studentMarkService;

        public StudentMarksController(IMediator mediator, IStudentMarksService studentMarkService)
        {
            _mediator = mediator;
            _studentMarkService = studentMarkService;
        }

        //Setup điểm
        [HttpPost("setup-by-class-id/{classId}")]
        public async Task<IActionResult> SetupByClassId(string classId)
        {
            var result = await _studentMarkService.SetupStudentMarkByClassIdAsync(classId);
            if(!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
 
        [HttpPost("create-from-student-test/{studentTestId}")]
        public async Task<IActionResult> CreateFromStudentTest(string studentTestId)
        {
            var command = new CreateStudentMarkFromStudentTestCommand { StudentTestId = studentTestId };
            var result = await _mediator.Send(command);

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
        [HttpPut("update-by-lecturer")]
        public async Task<IActionResult> UpdateByLecturer([FromBody] UpdateStudentMarksByLecturerCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }


        [HttpDelete("{studentMarkId}")]
        public async Task<IActionResult> DeleteStudentMark(string studentMarkId)
        {
            var command = new DeleteStudentMarkCommand { StudentMarkId = studentMarkId };
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("class/{classId}/assessment/{assessmentCriteriaId}")]
        public async Task<IActionResult> GetStudentMarksByClassAndAssessment(string classId, string assessmentCriteriaId)
        {
            var query = new GetStudentMarksByClassAndAssessmentQuery
            {
                ClassId = classId,
                AssessmentCriteriaId = assessmentCriteriaId
            };
            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentMarksByStudentId(string studentId)
        {
            var query = new GetStudentMarksByStudentIdQuery { StudentId = studentId };
            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
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

        [HttpGet("get-student-mark-by-class/{classId}")]
        public async Task<IActionResult> GetStudentMarksByClassId(string classId)
        {
            var result = await _studentMarkService.GetStudentMarkDetailDTOByClassIdAsync(classId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}