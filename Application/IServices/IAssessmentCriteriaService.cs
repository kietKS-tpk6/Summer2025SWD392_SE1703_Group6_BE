using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using MediatR;
namespace Application.IServices
{
    public interface IAssessmentCriteriaService
    {
        Task<OperationResult<AssessmentCriteria>> GetByIdAsync(string assessmentCriteriaId);
        Task<OperationResult<AssessmentCriteriaUpdateDto>> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand cmd);
        Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectId);
        Task<OperationResult<List<AssessmentCriteriaSetupDTO>>> SetupAssessmentCriteria(AssessmentCriteriaSetupCommand request);
        //Lỗi nên tạm comment - Kho
        ////KIỆT :HÀM CỦA KIỆT
        //Task<Dictionary<(string Category, string TestType), int>> GetRequiredTestCountsAsync(string syllabusId);
        ////KIỆT :HÀM CỦA KIỆT
        //Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, TestCategory category, TestType testType);
        OperationResult<bool> CheckDuplicateCategory(List<AssessmentCriteriaUpdateCommand> items);
        Task<OperationResult<List<AssessmentCriteriaUpdateDto>>> UpdateAssessmentCriteriaListAsync(List<AssessmentCriteriaUpdateCommand> items);
        OperationResult<bool> CheckRequiredTestCountRule(List<AssessmentCriteriaUpdateCommand> items);

    }
}
