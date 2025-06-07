using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using MediatR;
namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        public SyllabusScheduleController(IMediator mediator, ISyllabusScheduleService syllabusScheduleService)
        {
            _mediator = mediator;
            _syllabusScheduleService = syllabusScheduleService;
        }
        [HttpGet("max-slot/{syllabusId}")]
        public async Task<IActionResult> GetMaxSlotPerWeek(string syllabusId)
        {
            if (string.IsNullOrEmpty(syllabusId))
            {
                return BadRequest("SubjectId không được để trống.");
            }

            var maxSlot = await _syllabusScheduleService.GetMaxSlotPerWeekAsync(syllabusId);

            return Ok(maxSlot);
        }

    }
}
