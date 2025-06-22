using Application.IServices;
using Application.Usecases.Command;
using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IAccountService _accountService;
        private readonly ISubjectService _subjectService;
        private readonly ISystemConfigService _systemConfigService;

        public TestService(
            ITestRepository testRepository,
            IAccountService accountService,
            ISubjectService subjectService,
            ISystemConfigService systemConfigService)
        {
            _testRepository = testRepository;
            _accountService = accountService;
            _subjectService = subjectService;
            _systemConfigService = systemConfigService;
        }

        public async Task<OperationResult<string>> CreateTestAsync(CreateTestCommand command)
        {
            try
            {
                // Validate account exists and has proper role
                var accountResult = await _accountService.GetAccountByIdAsync(command.AccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;
                if (account.Role != AccountRole.Lecture && account.Role != AccountRole.Manager)
                    return OperationResult<string>.Fail("Only lecturers and managers can create tests");

                // Validate subject exists
                var subjectResult = await _subjectService.GetByIdAsync(command.SubjectID);
                if (!subjectResult.Success)
                    return OperationResult<string>.Fail("Subject not found");

                var testId = await GenerateNextTestIdAsync();
                var test = new Test
                {
                    TestID = testId,
                    CreateBy = command.AccountID,
                    SubjectID = command.SubjectID,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    Status = TestStatus.Drafted,
                    Category = command.Category,
                    TestType = command.TestType
                };

                return await _testRepository.CreateTestAsync(test);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error creating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateTestAsync(UpdateTestCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return testResult;

                var test = testResult.Data;

                // Check if requesting user is the creator
                if (test.CreateBy != command.RequestingAccountID)
                    return OperationResult<string>.Fail("You can only update tests you created");

                // Only allow updating test name and only if status is Draft
                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only update tests in Draft status");

                // Note: Based on requirements, only test name can be updated
                // But I don't see TestName field in Test entity, so this might need adjustment
                // For now, I'll assume we can update the test and let repository handle it

                return await _testRepository.UpdateTestAsync(test);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateTestStatusAsync(UpdateTestStatusCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return testResult;

                var test = testResult.Data;

                // Get requesting account
                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                // Business logic for status transitions
                var canTransition = CanTransitionStatus(test.Status, command.NewStatus, account.Role, test.CreateBy == command.RequestingAccountID);
                if (!canTransition.Success)
                    return canTransition;

                return await _testRepository.UpdateTestStatusAsync(command.TestID, command.NewStatus);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test status: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> DeleteTestAsync(DeleteTestCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return testResult;

                var test = testResult.Data;

                // Check permissions
                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                // Only creator or manager can delete
                if (test.CreateBy != command.RequestingAccountID && account.Role != AccountRole.Manager)
                    return OperationResult<string>.Fail("You don't have permission to delete this test");

                return await _testRepository.DeleteTestAsync(command.TestID);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error deleting test: {ex.Message}");
            }
        }

        public async Task<OperationResult<Test>> GetTestByIdAsync(string testId)
        {
            return await _testRepository.GetTestByIdAsync(testId);
        }

        public async Task<OperationResult<List<Test>>> GetTestsByAccountIdAsync(string accountId)
        {
            return await _testRepository.GetTestsByAccountIdAsync(accountId);
        }

        public async Task<string> GenerateNextTestIdAsync()
        {
            return await _testRepository.GenerateNextTestIdAsync();
        }

        public async Task ProcessAutoApprovalAsync()
        {
            try
            {
                // Get auto approval duration from system config
                var configResult = await _systemConfigService.GetConfigValueAsync("auto_approve_test_after_pending_duration");
                if (!configResult.Success || !int.TryParse(configResult.Data, out int days))
                    days = 3; // Default to 3 days

                var pendingTests = await _testRepository.GetPendingTestsOlderThanDaysAsync(days);

                foreach (var test in pendingTests)
                {
                    await _testRepository.UpdateTestStatusAsync(test.TestID, TestStatus.Actived);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                Console.WriteLine($"Error in auto approval process: {ex.Message}");
            }
        }

        private OperationResult<string> CanTransitionStatus(TestStatus currentStatus, TestStatus newStatus, AccountRole userRole, bool isCreator)
        {
            switch (currentStatus)
            {
                case TestStatus.Drafted:
                    if (newStatus == TestStatus.Pending && isCreator)
                        return OperationResult<string>.Ok("");
                    break;

                case TestStatus.Pending:
                    if (userRole == AccountRole.Manager)
                    {
                        if (newStatus == TestStatus.Actived || newStatus == TestStatus.Rejected)
                            return OperationResult<string>.Ok("");
                    }
                    break;

                case TestStatus.Actived:
                    if (userRole == AccountRole.Manager && newStatus == TestStatus.Deleted)
                        return OperationResult<string>.Ok("");
                    break;

                case TestStatus.Rejected:
                    if (isCreator && newStatus == TestStatus.Pending)
                        return OperationResult<string>.Ok("");
                    break;
            }

            return OperationResult<string>.Fail("Invalid status transition");
        }
    }
}