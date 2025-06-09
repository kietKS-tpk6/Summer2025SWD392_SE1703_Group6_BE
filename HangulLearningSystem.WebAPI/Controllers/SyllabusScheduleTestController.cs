using Microsoft.AspNetCore.Mvc;
using Application.Usecases.Command;
using MediatR;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services;
namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusScheduleTestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;

        public SyllabusScheduleTestController(IMediator mediator, ISyllabusScheduleTestService syllabusScheduleTestService)
        {
            _mediator = mediator;
            _syllabusScheduleTestService = syllabusScheduleTestService;
        }
        [HttpGet("check-completeness")]
        public async Task<IActionResult> CheckCompleteness([FromQuery] string syllabusId)
        {
            if (string.IsNullOrWhiteSpace(syllabusId))
            {
                return BadRequest("syllabusId is required.");
            }

            var result = await _syllabusScheduleTestService.CheckAddAssessmentCompletenessAsync(syllabusId);

            return Ok(new { message = result });
        }

    }
}
