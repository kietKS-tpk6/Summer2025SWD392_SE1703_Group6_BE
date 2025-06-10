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

                // Sử dụng decimal thay vì cast sang decimal từ float
                var qrUrl = _paymentService.GetQrCodeUrl(paymentId, (decimal)payment.Total);
                return Ok(new { qrCodeUrl = qrUrl });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("webhook-url")]
        public IActionResult GetWebhookUrl()
        {
            var webhookUrl = _paymentService.GetWebhookUrl();
            return Ok(new { webhookUrl });
        }
    }

    // Tạo controller riêng cho webhook để tránh conflict
    [Route("api/webhooks")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IMediator mediator, ILogger<WebhookController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPaymentWebhook([FromBody] TransactionDTO transaction)
        {
            try
            {
                _logger.LogInformation($"Received webhook for transaction: {transaction?.Id}");
                _logger.LogInformation($"Transaction content: {transaction?.Content}");
                _logger.LogInformation($"Transaction description: {transaction?.Description}");

                if (transaction == null)
                {
                    _logger.LogWarning("Received null transaction data");
                    return BadRequest(new { success = false, message = "Invalid transaction data" });
                }

                var command = new ProcessWebhookCommand { Transaction = transaction };
                var result = await _mediator.Send(command);

                _logger.LogInformation($"Webhook processed successfully: {result.Success}, Message: {result.Message}");

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}