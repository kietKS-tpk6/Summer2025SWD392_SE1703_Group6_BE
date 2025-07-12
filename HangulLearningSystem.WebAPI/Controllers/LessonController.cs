using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILessonService _lessonService;
        public LessonController(IMediator mediator, ILessonService lessonService)
        {
            _mediator = mediator;
            _lessonService = lessonService;
        }

        [HttpPost("create-from-schedule")]
        public async Task<IActionResult> CreateFromSchedule([FromBody] LessonCreateFromScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }



        [HttpPost("create-detail")]
        public async Task<IActionResult> Create([FromBody] LessonCreateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.Success && result.Data)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] LessonUpdateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.Success && result.Data)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("delete/{classLessonID}")]
        public async Task<IActionResult> DeleteLesson(string classLessonID)
        {
            var result = await _lessonService.DeleteLessonAsync(classLessonID);

            if (result.Success && result.Data)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
        [HttpDelete("delete-by-class-id/{classID}")]
        public async Task<IActionResult> DeleteLessonsByClassID(string classID)
        {
            var result = await _lessonService.DeleteLessonByClassIDAsync(classID);
            if (result.Success && result.Data)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        [HttpGet("get-by-class/{classID}")]
        public async Task<IActionResult> GetLessonsByClassID(string classID)
        {
            var result = await _lessonService.GetLessonContentByClassIdAsyn(classID);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("get-by-student")]
        public async Task<IActionResult> GetLessonsByStudentID([FromQuery] string studentID)
        {
            var result = await _lessonService.GetLessonsByStudentID(studentID);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("get-by-lecturer")]
        public async Task<IActionResult> GetLessonsByLecturerID([FromQuery] string lecturerID)
        {
            var result = await _lessonService.GetLessonsByLecturerID(lecturerID);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("get-detail/{classLessonID}")]
        public async Task<IActionResult> GetLessonDetailByLessonID(string classLessonID)
        {
            var result = await _lessonService.GetLessonDetailByLessonIDAsync(classLessonID);

            if (result.Success && result.Data != null)
                return Ok(result);

            return NotFound(result);
        }

    }
}
