using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.IServices;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITestService _testService;

        public TestController(IMediator mediator, ITestService testService)
        {
            _mediator = mediator;
            _testService = testService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestCommand command)
        {
            try
            {
                // Get AccountID from JWT token
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                command.AccountID = accountId;

                var result = await _mediator.Send(command);
                return Ok(new { message = "Test created successfully", testId = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{testId}")]
        public async Task<IActionResult> UpdateTest(string testId, [FromBody] UpdateTestCommand command)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                command.TestID = testId;
                command.RequestingAccountID = accountId;

                var result = await _mediator.Send(command);
                return Ok(new { message = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{testId}/status")]
        public async Task<IActionResult> UpdateTestStatus(string testId, [FromBody] UpdateTestStatusRequest request)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                var command = new UpdateTestStatusCommand
                {
                    TestID = testId,
                    NewStatus = request.Status,
                    RequestingAccountID = accountId
                };

                var result = await _mediator.Send(command);
                return Ok(new { message = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{testId}")]
        public async Task<IActionResult> DeleteTest(string testId)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                var command = new DeleteTestCommand
                {
                    TestID = testId,
                    RequestingAccountID = accountId
                };

                var result = await _mediator.Send(command);
                return Ok(new { message = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{testId}")]
        public async Task<IActionResult> GetTestById(string testId)
        {
            try
            {
                var result = await _testService.GetTestByIdAsync(testId);

                if (result.Success)
                    return Ok(result.Data);

                return NotFound(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("my-tests")]
        public async Task<IActionResult> GetMyTests()
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                var result = await _testService.GetTestsByAccountIdAsync(accountId);

                if (result.Success)
                    return Ok(result.Data);

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class UpdateTestStatusRequest
    {
        public TestStatus Status { get; set; }
    }
}