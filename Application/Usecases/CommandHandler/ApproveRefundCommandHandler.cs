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
    public class ApproveRefundCommandHandler : IRequestHandler<ApproveRefundCommand, RefundResponseDTO>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<ApproveRefundCommandHandler> _logger;

        public ApproveRefundCommandHandler(
            IPaymentService paymentService,
            ILogger<ApproveRefundCommandHandler> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<RefundResponseDTO> Handle(ApproveRefundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Processing refund approval for PaymentID: {request.PaymentID}");

                var result = await _paymentService.ApproveRefundAsync(request.PaymentID, request.ManagerID, request.ApprovalNote);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing refund approval for PaymentID: {request.PaymentID}");
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