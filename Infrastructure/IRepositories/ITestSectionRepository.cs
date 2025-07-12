using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.IRepositories
{
    public interface ITestSectionRepository
    {
        Task<bool> ExistsAsync(string testSectionId);
        Task<TestFormatType?> GetTestSectionTypeAsync(string testSectionId);
        Task<decimal?> GetScoreByTestSectionIdAsync(string testSectionId);
        Task<List<TestSection>> GetByTestIdAsync(string testId);
        
        //TA did it
        Task<OperationResult<string>> CreateTestSectionAsync(TestSection testSection);
        Task<OperationResult<TestSection>> GetTestSectionByIdAsync(string testSectionId);
        Task<OperationResult<List<TestSection>>> GetTestSectionsByTestIdAsync(string testId);
        Task<OperationResult<string>> UpdateTestSectionAsync(TestSection testSection);
        Task<OperationResult<string>> DeleteTestSectionAsync(string testSectionId);
        Task<string> GenerateNextTestSectionIdAsync();
        Task<List<TestSection>> GetByTestIDAndTypeAsync(string testID, TestFormatType type);
        Task<decimal> GetTotalScoreBySectionID(string testSectionID);

    }
}
