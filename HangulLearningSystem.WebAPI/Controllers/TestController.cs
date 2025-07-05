using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.IServices;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITestService _testService;
        private readonly IAccountService _accountService;
        private readonly ITestSectionService _testSectionService;
        private readonly ITestEventService _testEventService;

        public TestController(IMediator mediator, ITestService testService, IAccountService accountService, ITestSectionService testSectionService, ITestEventService 
            testEventService)
        {
            _mediator = mediator;
            _testService = testService;
            _accountService = accountService;
            _testSectionService = testSectionService;
            _testEventService = testEventService;

        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestCommand command)
        {
            try
            {
                // Get AccountID from JWT token
                /*var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return Unauthorized("Invalid token");

                command.AccountID = accountId;*/

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
        [HttpPut("update-status-fix")]
        public async Task<IActionResult> UpdateTestStatusFix(UpdateTestStatusFixCommand request)
        {
            var result = await _mediator.Send(request);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("{testId}")]
        public async Task<IActionResult> UpdateTest(string testId, [FromBody] UpdateTestCommand command)
        {
            try
            {
                //var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (string.IsNullOrEmpty(accountId))
                //    return Unauthorized("Invalid token");

                //command.TestID = testId;
                //command.RequestingAccountID = accountId;

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
                //var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (string.IsNullOrEmpty(accountId))
                //    return Unauthorized("Invalid token");
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    accountId = request.AccountID;
                }

                if (string.IsNullOrEmpty(accountId))
                    return BadRequest("AccountID is required either in JWT token or request body");

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
                //    var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //    if (string.IsNullOrEmpty(accountId))
                //        return Unauthorized("Invalid token");

                var command = new DeleteTestCommand
            {
                TestID = testId,
                //RequestingAccountID = accountId
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
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests([FromQuery] string? status = null, [FromQuery] string? createdBy = null)
        {
            try
            {
                OperationResult<List<Test>> result;

                if (!string.IsNullOrEmpty(status) || !string.IsNullOrEmpty(createdBy))
                {
                    result = await _testService.GetAllTestsWithFiltersAsync(status, createdBy);
                }
                else
                {
                    result = await _testService.GetAllTestsAsync();
                }

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                var dtos = result.Data.Select(test => new TestResponseDTO
                {
                    TestID = test.TestID,
                    CreateBy = test.CreateBy,
                    CreatedByName = test.Account?.Fullname ?? "Unknown",
                    SubjectID = test.SubjectID,
                    SubjectName = test.Subject?.SubjectName ?? "Unknown",
                    CreateAt = test.CreateAt,
                    UpdateAt = (DateTime)test.UpdateAt,
                    Status = test.Status,
                    Category = test.Category,
                    TestType = test.TestType,
                    TotalSections = 0 
                }).ToList();

                return Ok(new
                {
                    message = "Tests retrieved successfully",
                    total = dtos.Count,
                    data = dtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

       
      
        [HttpGet("all-with-sections")]
        public async Task<IActionResult> GetAllTestsWithSections([FromQuery] string? status = null)
        {
            try
            {
                OperationResult<List<Test>> result;

                if (!string.IsNullOrEmpty(status))
                {
                    result = await _testService.GetAllTestsWithFiltersAsync(status);
                }
                else
                {
                    result = await _testService.GetAllTestsAsync();
                }

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                var detailedDtos = new List<TestDetailDTO>();

                foreach (var test in result.Data)
                {
                    var sectionsResult = await _testSectionService.GetTestSectionsByTestIdAsync(test.TestID);
                    var sections = sectionsResult.Success ? sectionsResult.Data : new List<TestSection>();

                    var dto = new TestDetailDTO
                    {
                        TestID = test.TestID,
                        CreateBy = test.CreateBy,
                        CreatedByName = test.Account?.Fullname ?? "Unknown",
                        SubjectID = test.SubjectID,
                        SubjectName = test.Subject?.SubjectName ?? "Unknown",
                        CreateAt = test.CreateAt,
                        UpdateAt = (DateTime)test.UpdateAt,
                        Status = test.Status,
                        Category = test.Category,
                        TestType = test.TestType,
                        TestSections = sections.Select(s => new TestSectionResponseDTO
                        {
                            TestSectionID = s.TestSectionID,
                            TestID = s.TestID,
                            Context = s.Context,
                            ImageURL = s.ImageURL,
                            AudioURL = s.AudioURL,
                            TestSectionType = s.TestSectionType,
                            Score = s.Score,
                            IsActive = s.IsActive
                        }).ToList()
                    };

                    detailedDtos.Add(dto);
                }

                return Ok(new
                {
                    message = "Tests with sections retrieved successfully",
                    total = detailedDtos.Count,
                    data = detailedDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("pending-for-approval")]
        public async Task<IActionResult> GetPendingTestsForApproval()
        {
            try
            {
                //var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (!string.IsNullOrEmpty(accountId))
                //{
                //    var accountResult = await _accountService.GetAccountByIdAsync(accountId);
                //    if (!accountResult.Success || accountResult.Data.Role != AccountRole.Manager)
                //    {
                //        return Forbid("Only managers can access pending tests for approval");
                //    }
                //}

                var result = await _testService.GetAllTestsWithFiltersAsync("Pending");

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                var dtos = result.Data.Select(test => new TestResponseDTO
                {
                    TestID = test.TestID,
                    CreateBy = test.CreateBy,
                    CreatedByName = test.Account?.Fullname ?? "Unknown",
                    SubjectID = test.SubjectID,
                    SubjectName = test.Subject?.SubjectName ?? "Unknown",
                    CreateAt = test.CreateAt,
                    UpdateAt = (DateTime)test.UpdateAt,
                    Status = test.Status,
                    Category = test.Category,
                    TestType = test.TestType
                }).ToList();

                return Ok(new
                {
                    message = "Pending tests retrieved successfully",
                    total = dtos.Count,
                    data = dtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    

    public class UpdateTestStatusRequest
    {
        public TestStatus Status { get; set; }
        public string AccountID { get; set; } 
    }

    [HttpGet("advanced-search")]
        public async Task<IActionResult> GetTestsWithAdvancedFilters(
    [FromQuery] AssessmentCategory? category = null,
    [FromQuery] string? subjectId = null,
    [FromQuery] TestType? testType = null,
    [FromQuery] TestStatus? status = null)
        {
            try
            {
                var result = await _testService.GetTestsWithAdvancedFiltersAsync(category, subjectId, testType, status);

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                var dtos = result.Data.Select(test => new TestResponseDTO
                {
                    TestID = test.TestID,
                    CreateBy = test.CreateBy,
                    CreatedByName = test.Account?.Fullname ?? "Unknown",
                    SubjectID = test.SubjectID,
                    SubjectName = test.Subject?.SubjectName ?? "Unknown",
                    CreateAt = test.CreateAt,
                    UpdateAt = (DateTime)test.UpdateAt,
                    Status = test.Status,
                    Category = test.Category,
                    TestType = test.TestType,
                }).ToList();

                return Ok(new
                {
                    message = "Tests retrieved successfully with advanced filters",
                    total = dtos.Count,
                    filters = new
                    {
                        category = category?.ToString(),
                        subjectId = subjectId,
                        testType = testType?.ToString(),
                        status = status?.ToString()
                    },
                    data = dtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}