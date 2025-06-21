using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
