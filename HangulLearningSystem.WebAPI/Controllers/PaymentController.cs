using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPaymentService _paymentService;

        public PaymentController(IMediator mediator, IPaymentService paymentService)
        {
            _mediator = mediator;
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
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

        [HttpGet("status/{paymentId}")]
        public async Task<IActionResult> CheckPaymentStatus(string paymentId)
        {
            try
            {
                var status = await _paymentService.CheckPaymentStatusAsync(paymentId);
                return Ok(status);
            }
            catch (System.ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("qr/{paymentId}")]
        public async Task<IActionResult> GetPaymentQRCode(string paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentAsync(paymentId);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }

                var qrUrl = _paymentService.GetQrCodeUrl(paymentId, payment.Total);
                return Ok(new { qrCodeUrl = qrUrl });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("history/{studentId}")]
        public async Task<IActionResult> GetPaymentHistory(string studentId)
        {
            try
            {
                var history = await _paymentService.GetPaymentsForStudentAsync(studentId);
                return Ok(history);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }


 
}