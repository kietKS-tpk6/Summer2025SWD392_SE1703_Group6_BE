using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _systemConfigRepository;

        public SystemConfigService(ISystemConfigRepository systemConfigRepository)
        {
            _systemConfigRepository = systemConfigRepository;
        }

        public async Task<OperationResult<SystemConfig>> GetConfig(string key)
        {
            return await _systemConfigRepository.GetConfig(key);
        }
        public async Task<OperationResult<bool>> UpdateSystemConfigAsync(UpdateSystemConfigCommand request)
        {
            var systemConfig = await _systemConfigRepository.GetConfig(request.KeyToUpdate);
            if (!systemConfig.Success || systemConfig.Data == null)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("cấu hình"));
            }
            systemConfig.Data.Value = request.Value;
            systemConfig.Data.UpdatedAt = DateTime.UtcNow.AddHours(7);
            return await _systemConfigRepository.UpdateSystemConfigAsync(systemConfig.Data);
        }
    }
}
