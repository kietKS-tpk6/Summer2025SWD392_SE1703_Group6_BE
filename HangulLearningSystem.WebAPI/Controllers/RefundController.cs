// RefundController.cs
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPaymentService _paymentService;

        public RefundController(IMediator mediator, IPaymentService paymentService)
        {
            _mediator = mediator;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Check if a payment is eligible for refund
        /// </summary>
        /// <param name="paymentId">Payment ID to check</param>
        /// <param name="studentId">Student ID making the request</param>
        /// <returns>Refund eligibility information</returns>
        [HttpGet("eligibility/{paymentId}")]
        [Authorize] // Add authorization as needed
        public async Task<IActionResult> CheckRefundEligibility(string paymentId, [FromQuery] string studentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentId))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                var eligibility = await _paymentService.CheckRefundEligibilityAsync(paymentId, studentId);

                if (eligibility.IsEligible)
                {
                    return Ok(eligibility);
                }
                else
                {
                    return BadRequest(eligibility);
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Request a refund for a payment
        /// </summary>
        /// <param name="command">Refund request details</param>
        /// <returns>Refund request result</returns>
        [HttpPost("request")]
        [Authorize] // Add authorization as needed
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.PaymentID))
                {
                    return BadRequest(new { message = "Payment ID is required" });
                }

                if (string.IsNullOrWhiteSpace(command.StudentID))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Approve a refund request (Manager only)
        /// </summary>
        /// <param name="paymentId">Payment ID to approve refund for</param>
        /// <param name="command">Approval details</param>
        /// <returns>Approval result</returns>
        [HttpPost("approve/{paymentId}")]
        [Authorize(Roles = "Manager")] // Restrict to managers only
        public async Task<IActionResult> ApproveRefund(string paymentId, [FromBody] ApproveRefundCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId))
                {
                    return BadRequest(new { message = "Payment ID is required" });
                }

                command.PaymentID = paymentId;

                if (string.IsNullOrWhiteSpace(command.ManagerID))
                {
                    return BadRequest(new { message = "Manager ID is required" });
                }

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Get pending refund requests (Manager only)
        /// </summary>
        /// <returns>List of pending refund requests</returns>
        [HttpGet("pending")]
        [Authorize(Roles = "Manager")] // Restrict to managers only
        public async Task<IActionResult> GetPendingRefundRequests()
        {
            try
            {
                var pendingRequests = await _paymentService.GetPendingRefundRequestsAsync();
                return Ok(pendingRequests);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Get refund history
        /// </summary>
        /// <param name="studentId">Optional: Student ID to filter by (for student view)</param>
        /// <returns>List of refund history</returns>
        [HttpGet("history")]
        [Authorize] // Add authorization as needed
        public async Task<IActionResult> GetRefundHistory([FromQuery] string studentId = null)
        {
            try
            {
                var history = await _paymentService.GetRefundHistoryAsync(studentId);
                return Ok(history);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
    }
}

// Alternative: Add refund endpoints to existing PaymentController
// Add these methods to your existing PaymentController:

/*
[HttpGet("refund/eligibility/{paymentId}")]
public async Task<IActionResult> CheckRefundEligibility(string paymentId, [FromQuery] string studentId)
{
    // Same implementation as above
}

[HttpPost("refund/request")]
public async Task<IActionResult> RequestRefund([FromBody] RefundRequestCommand command)
{
    // Same implementation as above
}

[HttpPost("refund/approve/{paymentId}")]
public async Task<IActionResult> ApproveRefund(string paymentId, [FromBody] ApproveRefundCommand command)
{
    // Same implementation as above
}

[HttpGet("refund/pending")]
public async Task<IActionResult> GetPendingRefundRequests()
{
    // Same implementation as above
}

[HttpGet("refund/history")]
public async Task<IActionResult> GetRefundHistory([FromQuery] string studentId = null)
{
    // Same implementation as above
}
*/