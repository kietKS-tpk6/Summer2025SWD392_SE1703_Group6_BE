using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Infrastructure.IRepositories
{
    public interface IDashboardManagerRepository
    {
        Task<OperationResult<ManagerSidebarRightDTO>> GetDataForSidebarRightAsync();
        Task<OperationResult<ManagerDashboardOverviewDTO>> GetOverviewAsync();
    }
}
