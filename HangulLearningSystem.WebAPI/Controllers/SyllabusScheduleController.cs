using Application.Usecases.Command;
using MediatR;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Domain.Entities;
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
        [HttpPost("create-syllabus-schedule")]
        public async Task<IActionResult> CreateSyllabusSchedule([FromBody] SyllabusScheduleCreateCommand command, CancellationToken cancellationToken)
        {
            

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        //[HttpPut("update-syllabus-schedule")]
        //public async Task<IActionResult> updateSyllabusSchedule([FromBody] SyllabusScheduleUpdateCommand command, CancellationToken cancellationToken)
        //{


        //    var result = await _mediator.Send(command, cancellationToken);
        //    return Ok(result);
        //}
        [HttpPut("bulk-update")]
        public async Task<IActionResult> UpdateBulkSchedule([FromBody] UpdateSyllabusScheduleListCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("max-slot/{subjectId}")]
        public async Task<IActionResult> GetMaxSlotPerWeek(string subjectId)
        {
            if (string.IsNullOrEmpty(subjectId))
        {
                return BadRequest("SubjectId không được để trống.");
            }

            var maxSlot = await _syllabusScheduleService.GetMaxSlotPerWeekAsync(subjectId);

            return Ok(maxSlot);
        }
        [HttpGet("get-schedule-by-subject")]
        public async Task<IActionResult> GetScheduleBySubject(string subject, int? week)
        {
            // Validation tại controller
            if (string.IsNullOrEmpty(subject))
            {
                return BadRequest("SubjectId không được để trống.");
            }

            // Chỉ validate week khi có giá trị
            if (week.HasValue && week.Value < 1)
            {
                return BadRequest("Week phải lớn hơn 0.");
            }

            try
            {
                var schedules = await _syllabusScheduleService.GetScheduleBySubjectAndWeekAsync(subject, week);

                if (schedules == null || schedules.Count == 0)
                {
                    var message = week.HasValue
                        ? $"Không tìm thấy schedule cho Subject: {subject}, Week: {week}"
                        : $"Không tìm thấy schedule cho Subject: {subject}";
                    return NotFound(message);
                }

                var responseMessage = week.HasValue
                    ? $"Lấy danh sách schedule thành công cho Subject: {subject}, Week: {week}"
                    : $"Lấy toàn bộ danh sách schedule thành công cho Subject: {subject}";

                return Ok(new
                {
                    Success = true,
                    Message = responseMessage,
                    Data = schedules,
                    Total = schedules.Count,
                    FilteredByWeek = week.HasValue
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách schedule",
                    Error = ex.Message
                });
            }
        }

       
        [HttpGet("ongoing-class/{classID}/schedules-basic")]
        public async Task<IActionResult> GetSchedulesBasicInfoByOngoingClassID(string classID)
        {
            if (string.IsNullOrWhiteSpace(classID))
                return BadRequest("ClassID không được để trống.");

            var result = await _syllabusScheduleService.GetScheduleResourcesByClassIdAsync(classID);

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }
        [HttpGet("resource/{syllabusScheduleID}")]
        public async Task<IActionResult> GetResourcesBySyllabusScheduleID(string syllabusScheduleID)
        {
            if (string.IsNullOrWhiteSpace(syllabusScheduleID))
                return BadRequest("SyllabusScheduleID không được để trống.");

            var result = await _syllabusScheduleService.GetResourcesByScheduleIDAsync(syllabusScheduleID);

            if (!result.Success)
                return NotFound(new { Success = false, Message = result.Message });

            return Ok(new
            {
                Success = true,
                Message = "Lấy thông tin Resources thành công.",
                Data = new
                {
                    SyllabusScheduleID = syllabusScheduleID,
                    Resources = result.Data
                }
            });
        }


    }
}
