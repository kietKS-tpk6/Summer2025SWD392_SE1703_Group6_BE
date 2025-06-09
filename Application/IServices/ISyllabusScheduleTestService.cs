using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;


namespace Application.IServices
{
    public interface ISyllabusScheduleTestService
    {
        Task<AssessmentCompletenessResultDTO> CheckAddAssessmentCompletenessAsync(string syllabusId);
        Task<List<(string Category, string TestType)>> GetAddedTestsAsync(string syllabusId);
        Task<bool> IsTestOverLimitAsync(string syllabusId, TestCategory category, TestType testType);

    }
}
