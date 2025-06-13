using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
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

                return Ok(new { success = true, message = result.Message });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}