using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public PaymentRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreatePaymentAsync(Payment payment)
        {
            try
            {
                _dbContext.Payment.Add(payment);
                await _dbContext.SaveChangesAsync();
                return "Payment created successfully";
            }
            catch (Exception ex)
            {
                return $"Error creating payment: {ex.Message}";
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(string paymentId)
        {
            return await _dbContext.Payment
                .Include(p => p.Account)
                .Include(p => p.Class)
                //.Include(p => p.Transaction)
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId);
        }

        public async Task<List<Payment>> GetPaymentsByAccountIdAsync(string accountId)
        {
            return await _dbContext.Payment
                .Include(p => p.Account)
                .Include(p => p.Class)
                //.Include(p => p.Transaction)
                .Where(p => p.AccountID == accountId)
                .OrderByDescending(p => p.DayCreate)
                .ToListAsync();
        }

        public async Task<string> UpdatePaymentAsync(Payment payment)
        {
            try
            {
                _dbContext.Payment.Update(payment);
                await _dbContext.SaveChangesAsync();
                return "Payment updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error updating payment: {ex.Message}";
            }
        }

        public async Task<bool> PaymentExistsAsync(string paymentId)
        {
            return await _dbContext.Payment
                .AnyAsync(p => p.PaymentID == paymentId);
        }

        public async Task<int> GetTotalPaymentsCountAsync()
        {
            return await _dbContext.Payment.CountAsync();
        }
        public async Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            try
            {
                return await _dbContext.Payment
                    .Include(p => p.Account)
                    .Include(p => p.Class)
                    .Where(p => p.Status == status)
                    .OrderByDescending(p => p.DayCreate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Payment>(); 
            }
        }
        public async Task<OperationResult<List<GetPaymentsForStudentDTO>>> GetPaymentsForStudentAsync(string studentId)
        {
            var result = await _dbContext.Payment
                .Include(p => p.Account)
                .Include(p => p.Class)
                .Where(p => p.AccountID == studentId && p.Status != PaymentStatus.Pending) 
                .OrderByDescending(p => p.DayCreate)
                .Select(p => new GetPaymentsForStudentDTO
                {
                    PaymentId = p.PaymentID,
                    ClassName = p.Class.ClassName,
                    Total = p.Total,
                    PaymentStatus = p.Status,
                    PaidAt = p.DayCreate
                })
                .ToListAsync();
            return OperationResult<List<GetPaymentsForStudentDTO>>.Ok(result, OperationMessages.RetrieveSuccess("lịch sử thanh toán"));
        }
        public async Task<OperationResult<List<PaymentTableRowDTO>>> GetPaymentForExcelAsync()
        {
            var paymentsQuery = _dbContext.Payment
              .Where(p => p.Status != PaymentStatus.Pending)
              .OrderByDescending(p => p.DayCreate);
            if (paymentsQuery == null) return OperationResult<List<PaymentTableRowDTO>>.Fail(OperationMessages.NotFound("hóa đơn thanh toán"));
            var result = await (
                from p in paymentsQuery
                join a in _dbContext.Accounts on p.AccountID equals a.AccountID into accJoin
                from a in accJoin.DefaultIfEmpty()
                join c in _dbContext.Class on p.ClassID equals c.ClassID into classJoin
                from c in classJoin.DefaultIfEmpty()
                select new PaymentTableRowDTO
                {
                    PaymentID = p.PaymentID,
                    StudentID = p.AccountID,
                    ClassID = p.ClassID,
                    StudentName = a != null ? a.FirstName + " " + a.LastName : "(Không rõ)",
                    ClassName = c != null ? c.ClassName : "(Không rõ)",
                    Amount = p.Total,
                    Status = p.Status.ToString(),
                    PaidAt = p.DayCreate,
                })
                .ToListAsync();
          return OperationResult<List<PaymentTableRowDTO>>.Ok(result, OperationMessages.RetrieveSuccess("hóa đơn thanh toán"));
        }
        public async Task<PaginatedResult<Payment>> GetPaymentsByStatusWithPaginationAsync(PaymentStatus status, int page, int pageSize)
        {
            try
            {
                var query = _dbContext.Payment
                    .Include(p => p.Account)
                    .Include(p => p.Class)
                    .Where(p => p.Status == status)
                    .OrderByDescending(p => p.DayCreate);

                var totalCount = await query.CountAsync();

                var payments = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResult<Payment>(payments, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                return new PaginatedResult<Payment>(new List<Payment>(), 0, page, pageSize);
            }
        }
    }
}