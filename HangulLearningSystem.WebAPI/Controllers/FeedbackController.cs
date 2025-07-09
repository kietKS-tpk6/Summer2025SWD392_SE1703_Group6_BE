using Application.DTOs;
using Application.Usecases.Command;
using Application.Usecases.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO createFeedbackDto)
        {
            try
            {
                var command = new CreateFeedbackCommand
                {
                    ClassID = createFeedbackDto.ClassID,
                    StudentID = createFeedbackDto.StudentID,
                    Rating = createFeedbackDto.Rating,
                    Comment = createFeedbackDto.Comment
                };

                var result = await _mediator.Send(command);
                return Ok(new { message = "Feedback created successfully", feedbackId = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating feedback", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("class/{classId}")]
        public async Task<IActionResult> GetFeedbacksByClass(string classId)
        {
            try
            {
                var query = new GetFeedbackByClassQuery { ClassID = classId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving feedbacks", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("student/{studentId}")]
        public async Task<IActionResult> GetFeedbacksByStudent(string studentId)
        {
            try
            {
                var query = new GetFeedbackByStudentQuery { StudentID = studentId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving feedbacks", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(string feedbackId)
        {
            try
            {
                var query = new GetFeedbackByIdQuery { FeedbackID = feedbackId };
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { message = "Feedback not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving feedback", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("summary/class/{classId}")]
        public async Task<IActionResult> GetFeedbackSummary(string classId)
        {
            try
            {
                var query = new GetFeedbackSummaryQuery { ClassID = classId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving feedback summary", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(string feedbackId, [FromBody] UpdateFeedbackDTO updateFeedbackDto)
        {
            try
            {
                var command = new UpdateFeedbackCommand
                {
                    FeedbackID = feedbackId,
                    Rating = updateFeedbackDto.Rating,
                    Comment = updateFeedbackDto.Comment
                };

                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { message = "Feedback not found" });
                }

                return Ok(new { message = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating feedback", error = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(string feedbackId)
        {
            try
            {
                var command = new DeleteFeedbackCommand { FeedbackID = feedbackId };
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { message = "Feedback not found" });
                }

                return Ok(new { message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting feedback", error = ex.Message });
            }
        }
    }
}