using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class SyllabusScheduleTestService : ISyllabusScheduleTestService
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestRepository;

        public SyllabusScheduleTestService(IAssessmentCriteriaService assessmentCriteriaService, ISyllabusScheduleTestRepository syllabusScheduleTestRepository)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
            _syllabusScheduleTestRepository = syllabusScheduleTestRepository;
        }

        public async Task<AssessmentCompletenessResultDTO> CheckAddAssessmentCompletenessAsync(string syllabusId)
        {
            var requiredTests = await _assessmentCriteriaService.GetRequiredTestCountsAsync(syllabusId);
            var addedTests = await _syllabusScheduleTestRepository.GetTestsBySyllabusIdAsync(syllabusId);

            var missingTests = new List<MissingTestDTO>();

            foreach (var req in requiredTests)
            {
                var countAdded = addedTests.Count(x => x.TestCategory == req.Key.Category && x.TestType == req.Key.TestType);

                if (countAdded < req.Value)
                {
                    missingTests.Add(new MissingTestDTO
                    {
                        Category = req.Key.Category,
                        TestType = req.Key.TestType,
                        MissingCount = req.Value - countAdded
                    });
                }
            }

            var result = new AssessmentCompletenessResultDTO();

            if (missingTests.Any())
            {
                result.Message = "Còn thiếu bài kiểm tra";
                result.MissingTests = missingTests;
            }
            else
            {
                result.Message = "Đã đủ bài kiểm tra";
            }

            return result;
        }

        public async Task<List<(string Category, string TestType)>> GetAddedTestsAsync(string syllabusId)
        {
            var added = await _syllabusScheduleTestRepository.GetTestsBySyllabusIdAsync(syllabusId);
            return added.Select(x => (x.TestCategory, x.TestType)).ToList();
        }

       
    }
}
