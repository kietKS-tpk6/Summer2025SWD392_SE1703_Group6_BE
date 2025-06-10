using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
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
        string GetQrCodeUrl(string paymentId, decimal amount);
        string GetWebhookUrl();
    }
}