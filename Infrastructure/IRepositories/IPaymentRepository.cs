using Application.DTOs;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface IPaymentRepository
    {
        Task<string> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(string paymentId);
        Task<List<Payment>> GetPaymentsByAccountIdAsync(string accountId);
        Task<string> UpdatePaymentAsync(Payment payment);
        Task<bool> PaymentExistsAsync(string paymentId);
        Task<int> GetTotalPaymentsCountAsync();
    }
}