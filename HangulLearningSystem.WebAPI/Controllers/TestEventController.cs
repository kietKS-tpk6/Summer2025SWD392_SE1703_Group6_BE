using Application.IServices;
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
        [HttpDelete("delete-by-class-id/{classId}")]
        public async Task<IActionResult> DeleteTestEventByClassID(string classId)
        {
            var result = await _testEventService.DeleteTestEventsByClassIDAsync(classId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
