using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
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
    }
}
