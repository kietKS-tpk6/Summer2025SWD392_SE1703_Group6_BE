using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;


namespace Application.IServices
{
    public interface ISyllabusScheduleTestService
    {
        //Task<AssessmentCompletenessResultDTO> CheckAddAssessmentCompletenessAsync(string syllabusId);
        //Task<List<(string Category, string TestType)>> GetAddedTestsAsync(string syllabusId);
        //Task<bool> IsTestOverLimitAsync(string syllabusId, TestCategory category, TestType testType, int? excludeId = null);
        Domain.Enums.TestType? NormalizeTestType(string type, bool isRequired = true);
        Domain.Enums.TestCategory? NormalizeTestCategory(string category, bool isRequired = true);

        Task<OperationResult<SyllabusScheduleTest>> CreateAsync(SyllabusScheduleTest test);

        //Task<bool> AddTestToSyllabusAsync(AddTestSchedulesToSlotsCommand addTestSchedulesToSlotsCommand);
        //Task<bool> UpdateTestToSyllabusAsync(UpdateTestSchedulesToSlotsCommand updateTestSchedulesToSlotsCommand);

        //Task<bool> HasTestAsync(string syllabusScheduleId);
        //Task<bool> RemoveTestFromSlotAsyncs(string syllabusScheduleId);

        //Task<List<SyllabusScheduleTestDTO>> GetExamAddedAsync(string subject);
        Task<string> GenerateNewScheduleTestIdAsync();
        Task<bool> IsDuplicateTestTypeAsync(string assessmentCriteriaId, TestType testType);

    }
}
