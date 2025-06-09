using Application.Usecases.Command;
using Application.Common.Constants;

using Application.Usecases.CommandHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.IRepositories;
using Application.IServices;
using Application.DTOs;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISyllabusesService _syllabusesService;


        public SyllabusController(IMediator mediator, ISyllabusesService syllabusesService)
        {
            _mediator = mediator;
            _syllabusesService = syllabusesService;
        }

        //[Authorize(Roles = "Manager")]
        [HttpPost("create-syllabus")]
        public async Task<IActionResult> CreateSyllabus([FromBody] CreateSyllabusesCommand command, CancellationToken cancellationToken)
        {
            /*var accountId = User.FindFirst("AccountID")?.Value;

            if (accountId == null)
                return Unauthorized("Không tìm thấy accountID trong token.");
*/
            //command.AccountID = accountId; 

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

        //[Authorize(Roles = "Manager")]
        [HttpPut("update-syllabus")]
        public async Task<IActionResult> UpdateSyllabus([FromBody] UpdateSyllabusesCommand command, CancellationToken cancellationToken)
        {
            //var accountId = User.FindFirst("AccountID")?.Value;

            //if (accountId == null)
            //    return Unauthorized("Không tìm thấy accountID trong token.");

            //command.AccountID = accountId;

            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                return BadRequest("Tạo chương trình học thất bại");
            }
            return Ok(result);
        }

        // [Authorize(Roles = "Manager")]
        [HttpDelete("delete-syllabus/{id}")]
        public async Task<IActionResult> DeleteSyllabus(string id)
        {
            var success = await _syllabusesService.DeleteSyllabusById(id);

            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy syllabus với ID này" });
            }

            return Ok(new { message = "Xóa syllabus thành công" });
        }

        [HttpGet("get-syllabus-by-subject-id/{id}")]
        public async Task<IActionResult> getSyllabusBySubject(string id)
        {
            var success = await _syllabusesService.getSyllabusBySubjectID(id);

            if (success != null)
            {
                return Ok(success);

            }

            return NotFound(new { message = "Không tìm thấy syllabus với ID này" });
        }


    }
}
