using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardAnalyticsController : ControllerBase
    {
        private readonly IDashboardAnalyticsService _dashboardAnalyticsService;

        public DashboardAnalyticsController(IDashboardAnalyticsService dashboardAnalyticsService)
        {
            _dashboardAnalyticsService = dashboardAnalyticsService;
        }
        [HttpGet("payment-table")]
        public async Task<IActionResult> GetPaymentTable([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _dashboardAnalyticsService.GetPaginatedPaymentTableAsync(page,pageSize));
        }
        [HttpGet("lecturer-statistic")]
        public async Task<IActionResult> GetLecturerStatistic()
        {
            var result = await _dashboardAnalyticsService.GetLecturerStatisticsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("class-completion-statistic")]
        public async Task<IActionResult> GetDashboardData()
        {
            var result = await _dashboardAnalyticsService.GetClassCompletionStatsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
