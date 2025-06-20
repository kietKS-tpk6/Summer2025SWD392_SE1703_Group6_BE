using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure.IRepositories
{
    public interface ITestSectionRepository
    {
        Task<bool> ExistsAsync(string testSectionId);
        Task<TestFormatType?> GetTestSectionTypeAsync(string testSectionId);
    }
}
