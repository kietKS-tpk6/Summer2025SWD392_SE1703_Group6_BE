using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IMCQOptionRepository
    {
        Task<List<MCQOption>> GetByQuestionIdAsync(string questionId);
        Task AddRangeAsync(List<MCQOption> options);
        Task DeleteByQuestionIdAsync(string questionId);
        Task<List<string>> GetCorrectOptionIDsAsync(string questionID);
        
    }
}
