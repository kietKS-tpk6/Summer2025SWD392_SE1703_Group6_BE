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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TransactionRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateTransactionAsync(Transaction transaction)
        {
            _dbContext.Transaction.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction.TransactionID;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int transactionId)
        {
            return await _dbContext.Transaction
                .FirstOrDefaultAsync(t => t.TransactionID == transactionId);
        }

        public async Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Transaction
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}