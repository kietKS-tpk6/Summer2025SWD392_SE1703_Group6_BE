using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IMediator mediator, IEnrollmentService enrollmentService)
        {
            _mediator = mediator;
            _enrollmentService = enrollmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.Contains("successfully"))
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("my-classes/{studentId}")]
        public async Task<IActionResult> GetMyClasses(string studentId)
        {
            try
            {
                var classes = await _enrollmentService.GetMyClassesAsync(studentId);
                return Ok(classes);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("check-enrollment/{studentId}/{classId}")]
        public async Task<IActionResult> CheckEnrollment(string studentId, string classId)
        {
            try
            {
                var isEnrolled = await _enrollmentService.IsStudentEnrolledAsync(studentId, classId);
                return Ok(new { isEnrolled });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("class-enrollments/{classId}")]
        public async Task<IActionResult> GetClassEnrollments(string classId)
        {
            try
            {
                var count = await _enrollmentService.GetClassCurrentEnrollmentsAsync(classId);
                return Ok(new { currentEnrollments = count });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("active-student-count/{accountId}")]
        public async Task<IActionResult> GetActiveStudentCountByLecturer(string accountId)
        {
            var result = await _enrollmentService.CountActiveStudentsByLecturerAsync(accountId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}