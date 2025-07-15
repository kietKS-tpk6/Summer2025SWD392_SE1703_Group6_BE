using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
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
        Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<PaginatedResult<Payment>> GetPaymentsByStatusWithPaginationAsync(PaymentStatus status, int page, int pageSize);

        //Kho
        Task<OperationResult<List<GetPaymentsForStudentDTO>>> GetPaymentsForStudentAsync(string studentId);
        //Kho
        Task<OperationResult<List<PaymentTableRowDTO>>> GetPaymentForExcelAsync();
    }
}