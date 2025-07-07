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

        [HttpGet("eligibility/{paymentId}")]
        //[Authorize] 
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

        [HttpPost("request")]
        //[Authorize] 
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PaymentID))
                {
                    return BadRequest(new { message = "Payment ID is required" });
                }

                if (string.IsNullOrWhiteSpace(request.StudentID))
                {
                    return BadRequest(new { message = "Student ID is required" });
                }

                var result = await _paymentService.RequestRefundAsync(request.PaymentID, request.StudentID);

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

        [HttpPost("approve/{paymentId}")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveRefund(string paymentId, [FromBody] ApproveRefundRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId))
                {
                    return BadRequest(new { message = "Payment ID is required" });
                }

                if (string.IsNullOrWhiteSpace(request.ManagerID))
                {
                    return BadRequest(new { message = "Manager ID is required" });
                }

                var result = await _paymentService.ApproveRefundAsync(paymentId, request.ManagerID);

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

        [HttpGet("pending")]
        //[Authorize(Roles = "Manager")] // Restrict to managers only
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

        [HttpGet("history")]
        //[Authorize] 
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