using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface ISystemConfigRepository
    {
        Task<OperationResult<SystemConfig>> GetConfig(string key);
        Task<OperationResult<bool>> UpdateSystemConfigAsync(SystemConfig config);
    }
}
