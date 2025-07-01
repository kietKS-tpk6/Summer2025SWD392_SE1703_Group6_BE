using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestEventController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITestEventService _testEventService;
        public TestEventController(IMediator mediator, ITestEventService testEventService)
        {
            _mediator = mediator;
            _testEventService = testEventService;
        }
        [HttpPost("setup-test-event/{classId}")]
        public async Task<IActionResult> SetupTestEvent(string classId)
        {
            var result = await _testEventService.SetupTestEventsByClassIDAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPut("configure")] 
        public async Task<IActionResult> UpdateTestEvent([FromBody] UpdateTestEventCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateTestEventStatus([FromBody] UpdateStatusTestEventCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("delete-by-class-id/{classId}")]
        public async Task<IActionResult> DeleteTestEventByClassID(string classId)
        {
            var result = await _testEventService.DeleteTestEventsByClassIDAsync(classId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-by-id/{testEventID}")] 
        public async Task<IActionResult> GetTestEventByID(string testEventID)
        {
            var result = await _testEventService.GetTestEventWithLessonDTOByIDAsync(testEventID);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-by-class-id/{classId}")]
        public async Task<IActionResult> GetTestEventByClassID(string classId)
        {
            var result = await _testEventService.GetTestEventWithLessonsByClassIDAsync(classId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-by-student-id/{studentId}")]
        public async Task<IActionResult> GetTestEventByStudentID(string studentId)
        {
            var result = await _testEventService.GetTestEventByStudentIdAsync(studentId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("{testEventID}/assignment")]
        public async Task<IActionResult> GetAssignment(string testEventID)
        {
            var result = await _testEventService.GetTestAssignmentForStudentAsync(testEventID);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
    }
}
