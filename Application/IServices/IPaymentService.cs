using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentCommand command);
        Task<PaymentStatusDTO> CheckPaymentStatusAsync(string paymentId);
        Task<string> GenerateNextPaymentIdAsync();
        Task<WebhookResponseDTO> ProcessWebhookAsync(TransactionDTO transaction);
        Task<bool> UpdatePaymentStatusAsync(string paymentId, decimal amountReceived, int? transactionId = null);
        Task<Payment> GetPaymentAsync(string paymentId);
        string GetQrCodeUrl(string paymentId, decimal total);
        string GetWebhookUrl();
        Task<List<PaymentListItemDTO>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<PaginatedResult<PaymentListItemDTO>> GetPaymentsByStatusWithPaginationAsync(PaymentStatus status, int page, int pageSize);


        Task<RefundEligibilityDTO> CheckRefundEligibilityAsync(string paymentId, string studentId);
        Task<RefundResponseDTO> RequestRefundAsync(string paymentId, string studentId);
        Task<RefundResponseDTO> ApproveRefundAsync(string paymentId, string managerId);
        Task<List<RefundListItemDTO>> GetPendingRefundRequestsAsync();
        Task<List<RefundListItemDTO>> GetRefundHistoryAsync(string studentId = null);

        Task<OperationResult<List<GetPaymentsForStudentDTO>>> GetPaymentsForStudentAsync(string studentId);
    }
}
