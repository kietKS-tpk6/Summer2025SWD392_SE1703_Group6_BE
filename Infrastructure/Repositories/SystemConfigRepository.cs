using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SystemConfigRepository : ISystemConfigRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;
        public SystemConfigRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<SystemConfig>> GetConfig(string key)
        {
            var config = await _dbContext.SystemConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Key == key && c.IsActive);

            if (config == null)
            {
                return OperationResult<SystemConfig>.Fail(
                    OperationMessages.NotFound("cấu hình"));
            }

            return OperationResult<SystemConfig>.Ok(
                config,
                OperationMessages.RetrieveSuccess("cấu hình"));
        }
        public async Task<OperationResult<bool>> UpdateSystemConfigAsync(SystemConfig config)
        {
            _dbContext.SystemConfig.Update(config);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0
                ? OperationResult<bool>.Ok(true, OperationMessages.UpdateSuccess("cấu hình"))
                : OperationResult<bool>.Fail(OperationMessages.UpdateFail("cấu hình"));
        }
    }
}
