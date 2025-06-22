using Application.Usecases.Command;
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
    public class TestSectionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITestSectionService _testSectionService;

        public TestSectionController(IMediator mediator, ITestSectionService testSectionService)
        {
            _mediator = mediator;
            _testSectionService = testSectionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestSection([FromBody] CreateTestSectionCommand command)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                command.RequestingAccountID = accountId;

                var result = await _mediator.Send(command);
                return Ok(new { message = "Test section created successfully", testSectionId = result });
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

        [HttpPut("{testSectionId}")]
        public async Task<IActionResult> UpdateTestSection(string testSectionId, [FromBody] UpdateTestSectionCommand command)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                command.TestSectionID = testSectionId;
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

        [HttpDelete("{testSectionId}")]
        public async Task<IActionResult> DeleteTestSection(string testSectionId)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                var command = new DeleteTestSectionCommand
                {
                    TestSectionID = testSectionId,
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

        [HttpGet("{testSectionId}")]
        public async Task<IActionResult> GetTestSectionById(string testSectionId)
        {
            try
            {
                var result = await _testSectionService.GetTestSectionByIdAsync(testSectionId);

                if (result.Success)
                    return Ok(result.Data);

                return NotFound(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-test/{testId}")]
        public async Task<IActionResult> GetTestSectionsByTestId(string testId)
        {
            try
            {
                var result = await _testSectionService.GetTestSectionsByTestIdAsync(testId);

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
}