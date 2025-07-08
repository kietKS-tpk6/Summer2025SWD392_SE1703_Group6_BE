using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class DashboardManagerService : IDashboardManagerService
    {
        private readonly IDashboardManagerRepository _dashboardManagerRepository;
        public DashboardManagerService(IDashboardManagerRepository dashboardManagerRepository)
        {
            _dashboardManagerRepository = dashboardManagerRepository;
        }
        public async Task<OperationResult<ManagerSidebarRightDTO>> GetDataForSidebarRightAsync()
        {
            return await _dashboardManagerRepository.GetDataForSidebarRightAsync();
        }
        public async Task<OperationResult<ManagerDashboardOverviewDTO>> GetOverviewAsync()
        {
            return await _dashboardManagerRepository.GetOverviewAsync();
        }
        public async Task<OperationResult<List<ManagerAlertTaskDTO>>> GetAlertTasksAsync()
        {
            return await _dashboardManagerRepository.GetAlertTasksAsync();
        }
    }
}
