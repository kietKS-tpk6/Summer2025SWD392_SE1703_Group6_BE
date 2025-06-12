using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
namespace HangulLearningSystem.WebAPI.Controllers
{
    public class LessonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILessonService _lessonService;
        public LessonController(IMediator mediator, ILessonService lessonService)
        {
            _mediator = mediator;
            _lessonService = lessonService;
        }

        //[HttpPost("create-from-schedule")]
        //public async Task<IActionResult> CreateFromSchedule([FromBody] LessonCreateFromScheduleCommand command, CancellationToken cancellationToken)
        //{
        //        var result = await _mediator.Send(command, cancellationToken);
        //        if(result == OperationMessages.CreateSuccess)
        //        {
        //            return Ok(OperationMessages.CreateSuccess);
        //        }
        //        else return BadRequest(result);
        //}


        [HttpPost("create-detail")]
        public async Task<IActionResult> Create([FromBody] LessonCreateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.CreateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail);
            }
        }
        [HttpPut("update")]
       
        public async Task<IActionResult> Update([FromBody] LessonUpdateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.UpdateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.UpdateFail);
            }
        }
        [HttpDelete("delete/{classLessonID}")]
        public async Task<IActionResult> DeleteLesson(string classLessonID)
        {
            var result = await _lessonService.DeleteLessonAsync(classLessonID);
            if (result)
            {
                return Ok(OperationMessages.DeleteSuccess);
            }
            else
            {
                return NotFound(OperationMessages.DeleteFail);
            }
        }
        [HttpGet("get-by-class")]
        public async Task<IActionResult> GetLessonsByClassID([FromQuery] string classID)
        {
            try
            {
                var result = await _lessonService.GetLessonsByClassID(classID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-student")]
        public async Task<IActionResult> GetLessonsByStudentID([FromQuery] string studentID)
        {
            try
            {
                var result = await _lessonService.GetLessonsByStudentID(studentID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-lecturer")]
        public async Task<IActionResult> GetLessonsByLecturerID([FromQuery] string lecturerID)
        {
            try
            {
                var result = await _lessonService.GetLessonsByLecturerID(lecturerID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }
        [HttpGet("get-detail/{classLessonID}")]
        public async Task<IActionResult> GetLessonDetailByLessonID(string classLessonID)
        {
            var lessonDetail = await _lessonService.GetLessonDetailByLessonIDAsync(classLessonID);

            if (lessonDetail == null)
            {
                return NotFound(OperationMessages.NotFound);
            }

            return Ok(lessonDetail);
        }
        [ApiController]
        [Route("api/[controller]")]
        public class LessonsController : ControllerBase
        {
            private readonly IMediator _mediator;

            public LessonsController(IMediator mediator)
            {
                _mediator = mediator;
            }

            
        }

    }
}
