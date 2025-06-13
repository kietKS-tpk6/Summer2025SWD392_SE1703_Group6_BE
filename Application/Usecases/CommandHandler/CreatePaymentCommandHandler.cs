using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentResponseDTO>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<PaymentResponseDTO> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            return await _paymentService.CreatePaymentAsync(request);
        }
    }
}