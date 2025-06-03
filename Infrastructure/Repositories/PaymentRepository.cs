using Domain.Entities;
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
                .Include(p => p.Transaction)
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId);
        }

        public async Task<List<Payment>> GetPaymentsByAccountIdAsync(string accountId)
        {
            return await _dbContext.Payment
                .Include(p => p.Class)
                .Include(p => p.Transaction)
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
    }
}