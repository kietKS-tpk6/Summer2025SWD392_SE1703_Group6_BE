using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class ApproveRefundCommand : IRequest<RefundResponseDTO>
    {
        public string PaymentID { get; set; }
        public string ManagerID { get; set; }
        public string ApprovalNote { get; set; }
    }
}