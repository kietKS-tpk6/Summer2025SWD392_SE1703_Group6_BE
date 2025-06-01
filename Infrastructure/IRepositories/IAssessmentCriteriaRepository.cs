using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
namespace Infrastructure.IRepositories
{
    public interface IAssessmentCriteriaRepository
    {
        Task<List<AssessmentCriteria>> GetAllAsync();
        Task<AssessmentCriteria?> GetAsync(string id);
        Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria);
        Task<bool> UpdateAsync(AssessmentCriteria assessmentCriteria);
        Task<bool> DeleteAsync(AssessmentCriteria assessmentCriteria);
        Task<int> CountAsync();
    }
}
