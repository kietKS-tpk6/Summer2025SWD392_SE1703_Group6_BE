using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface ITransactionRepository
    {
        Task<int> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> GetTransactionByIdAsync(int transactionId);
        Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}