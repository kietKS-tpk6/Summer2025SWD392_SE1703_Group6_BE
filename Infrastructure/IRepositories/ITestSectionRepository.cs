using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
