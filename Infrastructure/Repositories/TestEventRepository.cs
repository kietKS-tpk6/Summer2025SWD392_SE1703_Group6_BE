using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TestEventRepository: ITestEventRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TestEventRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<bool>> CreateTestEventForCreateClassAsync(TestEvent testEvent)
        {
            if (testEvent == null)
                return OperationResult<bool>.Fail("buổi kiểm tra không hợp lệ");

            _dbContext.TestEvent.Add(testEvent);
            await _dbContext.SaveChangesAsync();

            return OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("buổi kiểm tra"));
        }

        public async Task<OperationResult<int>> CountTestEventAsync()
        {
            var count = await _dbContext.TestEvent.CountAsync();
            return OperationResult<int>.Ok(count);
        }
    }
}
