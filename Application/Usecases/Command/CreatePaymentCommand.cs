using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreatePaymentCommand : IRequest<PaymentResponseDTO>
    {
        public string AccountID { get; set; }
        public string ClassID { get; set; }
        public string Description { get; set; }
    }
}