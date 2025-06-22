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
                var accountResult = await _accountService.GetAccountByIdAsync(command.AccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;
                if (account.Role != AccountRole.Lecture && account.Role != AccountRole.Manager)
                    return OperationResult<string>.Fail("Only lecturers and managers can create tests");

                var subject = await _subjectService.GetSubjectByIdAsync(command.SubjectID);
                if (subject == null)
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
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                if (test.CreateBy != command.RequestingAccountID)
                    return OperationResult<string>.Fail("You can only update tests you created");

                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only update tests in Draft status");

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
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                var canTransition = CanTransitionStatus(test.Status, command.NewStatus, account.Role, test.CreateBy == command.RequestingAccountID);
                if (!canTransition.Success)
                    return OperationResult<string>.Fail(canTransition.Message);

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
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

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
                var configResult = await _systemConfigService.GetConfig("auto_approve_test_after_pending_duration");
                int days = 3; 

                if (configResult.Success && configResult.Data != null && !string.IsNullOrEmpty(configResult.Data.Value))
                {
                    if (!int.TryParse(configResult.Data.Value, out days))
                        days = 3; 
                }

                var pendingTests = await _testRepository.GetPendingTestsOlderThanDaysAsync(days);

                foreach (var test in pendingTests)
                {
                    await _testRepository.UpdateTestStatusAsync(test.TestID, TestStatus.Actived);
                }
            }
            catch (Exception ex)
            {
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