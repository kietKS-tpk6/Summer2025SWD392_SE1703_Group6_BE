using System.Security.Claims;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        public class StudentTestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStudentTestService _studentTestService;
        private readonly ITestService _testService;

        public StudentTestsController(IMediator mediator, IStudentTestService studentTestService, ITestService testService)
        {
            _mediator = mediator;
            _studentTestService = studentTestService;
            _testService = testService;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitStudentTest([FromBody] SubmitStudentTestCommand command)
        {
            // Lấy AccountID từ token
            var accountIdClaim = User.FindFirst("AccountID") ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (accountIdClaim == null)
            {
                return Unauthorized("Không tìm thấy AccountID trong token");
            }

            // Gán AccountID vào command
            command.StudentId = accountIdClaim.Value;

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Lecture")]
        [HttpPut("writing/grade")]
        public async Task<IActionResult> GradeWritingAnswer([FromBody] GradeWritingAnswerCommand command)
        {
            var accountId = User.FindFirst("AccountID")?.Value;

            if (string.IsNullOrEmpty(accountId))
                return Unauthorized("Không xác định được tài khoản từ token.");

            command.GraderAccountID = accountId;

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("list-test-results/{testEventId}")]
        public async Task<IActionResult> GetStudentTestResults(string testEventId)
        {
            var result = await _testService.GetListStudentTestResultsByTestEventAsync(testEventId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
        [HttpGet("test-results/{testEventId}")]
        public async Task<IActionResult> GetStudentTestResults(string testEventId, [FromQuery] string accountId)
        {
            var result = await _testService.GetStudentTestResultsByTestEventAsync(testEventId, accountId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
        [HttpGet("result-by-student-test/{studentTestID}")]
        public async Task<IActionResult> GetStudentTestResultByStudentTestID(string studentTestID)
        {
            var command = new GetStudentTestResultByStudentTestIDQueryCommand(studentTestID);
            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("pending-writing-count/{lecturerId}")]
        public async Task<IActionResult> CountPendingWrittenGrading(string lecturerId)
        {
            var result = await _studentTestService.CountPendingWrittenGradingAsync(lecturerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
