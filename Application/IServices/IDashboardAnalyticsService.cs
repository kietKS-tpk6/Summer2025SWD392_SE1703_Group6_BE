using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;

namespace Application.IServices
{
    public interface IDashboardAnalyticsService
    {
        Task<OperationResult<PagedResult<PaymentTableRowDTO>>> GetPaginatedPaymentTableAsync(int page, int pageSize);
        Task<OperationResult<List<LecturerStatisticsDTO>>> GetLecturerStatisticsAsync();
        Task<OperationResult<List<ClassCompletionStatsDTO>>> GetClassCompletionStatsAsync();
        Task<OperationResult<List<StudentPerformanceInClassDTO>>> GetStudentPerformanceInClassAsync(string classId);

    }
}
