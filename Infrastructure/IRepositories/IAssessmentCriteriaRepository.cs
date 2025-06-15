using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.IRepositories
{
    public interface IAssessmentCriteriaRepository
    {
        Task<OperationResult<List<AssessmentCriteriaSetupDTO>>> CreateManyAsync(List<AssessmentCriteria> list);
        Task<OperationResult<AssessmentCriteria>> UpdateAsync(AssessmentCriteria assessmentCriteria);

        Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectID);
        Task<List<AssessmentCriteria>> GetAllAsync();
         Task<int> CountAsync();
        Task<OperationResult<AssessmentCriteria>> GetByIdAsync(string assessmentCriteriaId);
        Task<OperationResult<bool>> SoftDeleteAsync(string id);
        Task<OperationResult<bool>> SoftDeleteByIdsAsync(List<string> id);
        //KIỆT :HÀM CỦA KIỆT (Lỗi nên tạm comment - Kho)
        //Task<bool> IsTestDefinedInCriteriaAsync(string subjectID, string category, string testType);

    }
}
