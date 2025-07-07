using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class RefundRequestCommand : IRequest<RefundResponseDTO>
    {
        public string PaymentID { get; set; }
        public string StudentID { get; set; }
        public string Reason { get; set; }
    }
}