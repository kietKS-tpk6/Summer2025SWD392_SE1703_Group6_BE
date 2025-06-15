using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, WebhookResponseDTO>
    {
        private readonly IPaymentService _paymentService;

        public ProcessWebhookCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<WebhookResponseDTO> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            return await _paymentService.ProcessWebhookAsync(request.Transaction);
        }
    }
}