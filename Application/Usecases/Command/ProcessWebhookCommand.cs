using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class ProcessWebhookCommand : IRequest<WebhookResponseDTO>
    {
        public TransactionDTO Transaction { get; set; }
    }
}