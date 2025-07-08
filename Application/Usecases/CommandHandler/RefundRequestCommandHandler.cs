using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class RefundRequestCommandHandler : IRequestHandler<RefundRequestCommand, RefundResponseDTO>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<RefundRequestCommandHandler> _logger;

        public RefundRequestCommandHandler(
            IPaymentService paymentService,
            ILogger<RefundRequestCommandHandler> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<RefundResponseDTO> Handle(RefundRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Processing refund request for PaymentID: {request.PaymentID}");

                var result = await _paymentService.RequestRefundAsync(request.PaymentID, request.StudentID);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing refund request for PaymentID: {request.PaymentID}");
                return new RefundResponseDTO
                {
                    Success = false,
                    Message = ex.Message,
                    PaymentID = request.PaymentID
                };
            }
        }
    }
}