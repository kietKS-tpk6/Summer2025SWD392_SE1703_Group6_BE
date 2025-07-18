using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class DashboardAnalyticsService : IDashboardAnalyticsService
    {
        private readonly IDashboardAnalyticsRepository _dashboardAnalyticsRepository;
        public DashboardAnalyticsService(IDashboardAnalyticsRepository dashboardAnalyticsRepository)
        {
            _dashboardAnalyticsRepository = dashboardAnalyticsRepository;
        }
        public async Task<OperationResult<PagedResult<PaymentTableRowDTO>>> GetPaginatedPaymentTableAsync(int page, int pageSize)
        {
            return await _dashboardAnalyticsRepository.GetPaginatedPaymentTableAsync(page, pageSize);
        }
        public async Task<OperationResult<List<LecturerStatisticsDTO>>> GetLecturerStatisticsAsync()
        {
            return await _dashboardAnalyticsRepository.GetLecturerStatisticsAsync();
        }
        public async Task<OperationResult<List<ClassCompletionStatsDTO>>> GetClassCompletionStatsAsync()
        {
            return await _dashboardAnalyticsRepository.GetClassCompletionStatisticsAsync();
        }
    }
}
