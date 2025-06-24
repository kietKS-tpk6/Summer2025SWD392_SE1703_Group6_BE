using Application.IServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IMediator mediator, IAttendanceService attendanceService)
        {
            _mediator = mediator;
            _attendanceService = attendanceService;
        }
        [HttpPost("setup-attendace-by-class-id/{classId}")]
        public async Task<IActionResult> SetupAttendaceByClassID(string classId)
        {
            var result = await _attendanceService.SetupAttendaceByClassIdAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("get-by-class-id/{classId}")]
        public async Task<IActionResult> GetAttendaceByClassID(string classId)
        {
            var result = await _attendanceService.GetAttendanceAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("get-by-lesson-id/{lessonId}")]
        public async Task<IActionResult> GetAttendaceByLessonID(string lessonId)
        {
            var result = await _attendanceService.GetAttendanceByLessonIdAsync(lessonId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
