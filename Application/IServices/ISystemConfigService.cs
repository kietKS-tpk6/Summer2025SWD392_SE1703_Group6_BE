﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;

namespace Application.IServices
{
    public interface ISystemConfigService
    {
        Task<OperationResult<SystemConfig>> GetConfig(string key);
        Task<OperationResult<bool>> UpdateSystemConfigAsync(UpdateSystemConfigCommand request);
        Task<OperationResult<List<SystemConfig>>> GetListSystemConfigAsync();
    }
}
