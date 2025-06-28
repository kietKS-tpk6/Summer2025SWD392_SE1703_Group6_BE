using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class TestSectionService : ITestSectionService
    {
        private readonly ITestSectionRepository _repo;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly ITestRepository _testRepository;
        private readonly IAccountService _accountService;
        public TestSectionService(ITestSectionRepository repo, ITestSectionRepository testSectionRepository, ITestRepository testRepository, IAccountService accountService)
        {
            _repo = repo;
            _testSectionRepository = testSectionRepository;
            _testRepository = testRepository;
            _accountService = accountService;
        }

        public async Task<bool> IsTestSectionExistAsync(string testSectionId)
        {
            return await _repo.ExistsAsync(testSectionId);
        }
      
        public async Task<OperationResult<string>> ValidateSectionTypeMatchFormatAsync(string testSectionId, TestFormatType formatType)
        {
            var sectionType = await _repo.GetTestSectionTypeAsync(testSectionId);

            if (sectionType.ToString() != formatType.ToString())
            {
                return OperationResult<string>.Fail("TestSectionType không khớp với định dạng câu hỏi được tạo.");
            }

            return OperationResult<string>.Ok("Hợp lệ.");
        }
        public async Task<List<TestSection>> GetByTestIdAsync(string testId)
        {
            return await _repo.GetByTestIdAsync(testId);
        }
        public async Task<OperationResult<string>> CreateTestSectionAsync(CreateTestSectionCommand command)
        {
            try
            {
                // Validate test exists
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail("Test not found");

                var test = testResult.Data;

                // Check if user has permission (must be creator or manager)
                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success || accountResult.Data == null)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                if (test.CreateBy != command.RequestingAccountID &&
                    !string.Equals(account.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<string>.Fail("You don't have permission to update this test section");
                }

                var compatibilityCheck = ValidateTestSectionCompatibility(test.TestType, command.TestSectionType);

                if (!compatibilityCheck.Success)
                    return compatibilityCheck;

                // Only allow creating sections if test is in Draft status
                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only add sections to tests in Draft status");

                var testSectionId = await GenerateNextTestSectionIdAsync();
                var testSection = new TestSection
                {
                    TestSectionID = testSectionId,
                    TestID = command.TestID,
                    Context = command.Context,
                    ImageURL = command.ImageURL,
                    AudioURL = command.AudioURL,
                    TestSectionType = command.TestSectionType,
                    Score = (decimal)command.Score,
                    IsActive = true
                };

                return await _testSectionRepository.CreateTestSectionAsync(testSection);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error creating test section: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateTestSectionAsync(UpdateTestSectionCommand command)
        {
            try
            {
                var testSectionResult = await _testSectionRepository.GetTestSectionByIdAsync(command.TestSectionID);
                if (!testSectionResult.Success)
                    return OperationResult<string>.Fail(testSectionResult.Message);

                var testSection = testSectionResult.Data;

                // Get the test to check permissions and status
                var testResult = await _testRepository.GetTestByIdAsync(testSection.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail("Associated test not found");

                var test = testResult.Data;

                // Check permissions
                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success || accountResult.Data == null)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                if (test.CreateBy != command.RequestingAccountID &&
                    !string.Equals(account.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<string>.Fail("You don't have permission to update this test section");
                }


                // Only allow updates if test is in Draft status
                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only update sections of tests in Draft status");

                // Update fields
                if (!string.IsNullOrEmpty(command.Context))
                    testSection.Context = command.Context;

                if (command.ImageURL != null)
                    testSection.ImageURL = command.ImageURL;

                if (command.AudioURL != null)
                    testSection.AudioURL = command.AudioURL;

                if (command.Score.HasValue)
                    testSection.Score = (decimal)command.Score;

                return await _testSectionRepository.UpdateTestSectionAsync(testSection);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test section: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> DeleteTestSectionAsync(DeleteTestSectionCommand command)
        {
            try
            {
                var testSectionResult = await _testSectionRepository.GetTestSectionByIdAsync(command.TestSectionID);
                if (!testSectionResult.Success)
                    return OperationResult<string>.Fail(testSectionResult.Message); ;

                var testSection = testSectionResult.Data;

                // Get the test to check permissions
                var testResult = await _testRepository.GetTestByIdAsync(testSection.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail("Associated test not found");

                var test = testResult.Data;

                // Check permissions
                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success || accountResult.Data == null)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                if (test.CreateBy != command.RequestingAccountID &&
                    !string.Equals(account.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<string>.Fail("You don't have permission to delete this test section");
                }

                // Only allow deletion if test is in Draft status
                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only delete sections of tests in Draft status");

                return await _testSectionRepository.DeleteTestSectionAsync(command.TestSectionID);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error deleting test section: {ex.Message}");
            }
        }

        public async Task<OperationResult<TestSection>> GetTestSectionByIdAsync(string testSectionId)
        {
            return await _testSectionRepository.GetTestSectionByIdAsync(testSectionId);
        }

        public async Task<OperationResult<List<TestSection>>> GetTestSectionsByTestIdAsync(string testId)
        {
            return await _testSectionRepository.GetTestSectionsByTestIdAsync(testId);
        }

        public async Task<string> GenerateNextTestSectionIdAsync()
        {
            return await _testSectionRepository.GenerateNextTestSectionIdAsync();
        }

        private OperationResult<string> ValidateTestSectionCompatibility(TestType testType, TestFormatType sectionType)
        {
            // If test type is MCQ, section type must be Multiple or TrueFalse
            if (testType == TestType.MCQ)
            {
                if (sectionType != TestFormatType.Multiple && sectionType != TestFormatType.TrueFalse)
                    return OperationResult<string>.Fail("MCQ tests can only have Multiple Choice or True/False sections");
            }

            // If test type is Mix, any section type is allowed
            // Other test types can have any compatible section type based on business rules

            return OperationResult<string>.Ok("");
        }
    }
}
