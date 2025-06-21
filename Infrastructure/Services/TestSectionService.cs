using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
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
        public TestSectionService(ITestSectionRepository repo)
        {
            _repo = repo;
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
    }
}
