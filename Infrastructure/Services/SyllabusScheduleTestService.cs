using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class SyllabusScheduleTestService : ISyllabusScheduleTestService
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestRepository;
        private readonly ISyllabusScheduleRepository _syllabusScheduleRepository;


        public SyllabusScheduleTestService(IAssessmentCriteriaService assessmentCriteriaService, ISyllabusScheduleTestRepository syllabusScheduleTestRepository, ISyllabusScheduleRepository syllabusScheduleRepository)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
            _syllabusScheduleTestRepository = syllabusScheduleTestRepository;
            _syllabusScheduleRepository = syllabusScheduleRepository;
        }

        //public async Task<AssessmentCompletenessResultDTO> CheckAddAssessmentCompletenessAsync(string syllabusId)
        //{
        //    var requiredTests = await _assessmentCriteriaService.GetRequiredTestCountsAsync(syllabusId);
        //    var addedTests = await _syllabusScheduleTestRepository.GetTestsBySyllabusIdAsync(syllabusId);

        //    var missingTests = new List<MissingTestDTO>();

        //    foreach (var req in requiredTests)
        //    {
        //        var countAdded = addedTests.Count(x => x.TestCategory == req.Key.Category && x.TestType == req.Key.TestType);

        //        if (countAdded < req.Value)
        //        {
        //            missingTests.Add(new MissingTestDTO
        //            {
        //                Category = req.Key.Category,
        //                TestType = req.Key.TestType,
        //                MissingCount = req.Value - countAdded
        //            });
        //        }
        //    }

        //    var result = new AssessmentCompletenessResultDTO();

        //    if (missingTests.Any())
        //    {
        //        result.Message = "Còn thiếu bài kiểm tra";
        //        result.MissingTests = missingTests;
        //    }
        //    else
        //    {
        //        result.Message = "Đã đủ bài kiểm tra";
        //    }

        //    return result;
        //}


        public async Task<List<(string Category, string TestType)>> GetAddedTestsAsync(string syllabusId)
        {
            var added = await _syllabusScheduleTestRepository.GetTestsBySyllabusIdAsync(syllabusId);
            return added.Select(x => (x.TestCategory, x.TestType)).ToList();
        }
        //kiểm tra số lượng bài
        //public async Task<bool> IsTestOverLimitAsync(string subjectID, TestCategory category, TestType testType, int? excludeId = null)
        //{
        //    var requiredTestCounts = await _assessmentCriteriaService.GetRequiredTestCountsAsync(subjectID);
        //    var key = (category.ToString(), testType.ToString());
        //    if (!requiredTestCounts.TryGetValue(key, out var requiredCount))
        //        return false; // Không có yêu cầu => không vượt quá

        //    var addedTests = await _syllabusScheduleTestRepository.GetTestsBySyllabusIdAsync(subjectID);

        //    // Debug: In ra để xem dữ liệu
        //    Console.WriteLine($"Checking limit for: {category} - {testType}");
        //    Console.WriteLine($"Required count: {requiredCount}");
        //    Console.WriteLine($"ExcludeId: {excludeId}");

        //    var addedCount = addedTests.Count(t =>
        //    {
        //        var matchCategory = t.TestCategory == category.ToString();
        //        var matchType = t.TestType == testType.ToString();
        //        var notExcluded = (excludeId == null || t.ID != excludeId);

        //        Console.WriteLine($"Test ID: {t.ID}, Category: {t.TestCategory}, Type: {t.TestType}, Match: {matchCategory && matchType && notExcluded}");

        //        return matchCategory && matchType && notExcluded;
        //    });

        //    Console.WriteLine($"Added count: {addedCount}");
        //    Console.WriteLine($"Is over limit: {addedCount >= requiredCount}");

        //    return addedCount >= requiredCount;
        //}

        /// <summary>
        /// Hàm helper để chuẩn hóa string: trim và viết hoa chữ cái đầu
        /// </summary>
        /// <param name="input">String cần chuẩn hóa</param>
        /// <returns>String đã được chuẩn hóa</returns>
        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var normalized = input.Trim();
            if (normalized.Length > 0)
            {
                normalized = char.ToUpper(normalized[0]) + normalized.Substring(1).ToLower();
            }

            return normalized;
        }



        public Domain.Enums.TestCategory? NormalizeTestCategory(string category, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                if (isRequired)
                    throw new ArgumentException("Thể loại bài kiểm tra không được để trống.");
                return null;
            }

            var normalizedCategory = NormalizeString(category);

            if (normalizedCategory == null)
                return null;

            if (Enum.TryParse<Domain.Enums.TestCategory>(normalizedCategory, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Thể loại '{category}' không hợp lệ. Chỉ chấp nhận: Midterm, Final, FifteenMinutes.");
        }
        public Domain.Enums.TestType? NormalizeTestType(string type, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                if (isRequired)
                    throw new ArgumentException("Loại bài kiểm tra không được để trống.");
                return null;
            }

            var normalizedType = NormalizeString(type);

            if (normalizedType == null)
                return null;

            if (Enum.TryParse<Domain.Enums.TestType>(normalizedType, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Loại bài kiểm tra '{type}' không hợp lệ. Chỉ chấp nhận: MCQ, Writing, Speaking, Listening, Reading, Mix, Other.");
        }

        public async Task<bool> AddTestToSyllabusAsync(AddTestSchedulesToSlotsCommand addTestSchedulesToSlotsCommand)
        {
            var bien = new SyllabusScheduleTest();
            bien.SyllabusSchedulesID = addTestSchedulesToSlotsCommand.SyllabusScheduleID;
            bien.TestCategory = Enum.Parse<TestCategory>(addTestSchedulesToSlotsCommand.TestCategory, true);
            bien.TestType = Enum.Parse<TestType>(addTestSchedulesToSlotsCommand.TestType, true);
            bien.IsActive = true;
            return await _syllabusScheduleTestRepository.AddAsync(bien);
        }
        public async Task<bool> UpdateTestToSyllabusAsync(UpdateTestSchedulesToSlotsCommand updateTestSchedulesToSlotsCommand)
        {
            // Lấy record cần update theo ID
            var existingTest = await _syllabusScheduleTestRepository.GetByIdAsync(updateTestSchedulesToSlotsCommand.SyllabusScheduleTestsId);

            // Update các thuộc tính
            existingTest.ID = updateTestSchedulesToSlotsCommand.SyllabusScheduleTestsId;
            existingTest.TestCategory = Enum.Parse<TestCategory>(updateTestSchedulesToSlotsCommand.TestCategory, true);
            existingTest.TestType = Enum.Parse<TestType>(updateTestSchedulesToSlotsCommand.TestType, true);
            existingTest.IsActive = true;

            // Gọi UpdateAsync thay vì AddAsync
            return await _syllabusScheduleTestRepository.UpdateAsync(existingTest);
        }

        public async Task<bool> HasTestAsync(string syllabusScheduleId)
        {
            return await _syllabusScheduleTestRepository.HasTestAsync(syllabusScheduleId);
        }

        public async Task<bool> RemoveTestFromSlotAsyncs(string syllabusScheduleId)
        {
            return await _syllabusScheduleTestRepository.RemoveTestFromSlotAsyncs(syllabusScheduleId);
        }

        public async Task<List<SyllabusScheduleTestDTO>> GetExamAddedAsync(string subject)
        {
            // Lấy danh sách các schedule theo syllabus
            var allSchedules = await _syllabusScheduleRepository.GetSyllabusSchedulesBySyllabusIdAsync(subject);

            // Lọc ra các slot có HasTest = true và lấy SyllabusScheduleID
            var slotsHaveExam = allSchedules
                .Where(x => x.HasTest == true && x.IsActive == true)
                .Select(x => x.SyllabusScheduleID)
                .ToList();

            // Truyền danh sách slot vào hàm GetExamAddedToSyllabusAsync
            var examTests = await _syllabusScheduleTestRepository.GetExamAddedToSyllabusAsync(slotsHaveExam);

            // Trả về danh sách SyllabusScheduleTest
            return examTests;
        }


    }
}
