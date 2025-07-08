using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
namespace Infrastructure.IRepositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionBySectionId(string sectionId);
        Task<int> GetTotalQuestionCount();
        Task AddRangeAsync(IEnumerable<Question> questions);
        Task UpdateRangeAsync(IEnumerable<Question> questions);
        Task<Question?> GetByIdAsync(string questionId);
        Task UpdateAsync(Question question);
        Task<List<Question>> GetByIDsAsync(List<string> IDs);
        Task<List<Question>> GetByTestSectionIDsAsync(List<string> testSectionIDs);

    }
}
