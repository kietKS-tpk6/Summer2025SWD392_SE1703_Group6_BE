using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IChartService _chartService;
        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }
        [HttpGet("student-signup-monthly")]
        public async Task<IActionResult> GetStudentSignupMonthly()
        {
            var result = await _chartService.GetStudentSignupMonthlyAsync();
            return Ok(result);
        }
        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth()
        {
            var result = await _chartService.GetRevenueByMonthAsync();
            return Ok(result);
        }
        [HttpGet("class-count-by-subject")]
        public async Task<IActionResult> GetClassCountBySubject()
        {
            var result = await _chartService.GetClassCountBySubjectAsync();
            return Ok(result);
        }
        [HttpGet("class-status-distribution")]
        public async Task<IActionResult> GetClassStatusDistribution()
        {
            var result = await _chartService.GetClassStatusDistributionAsync();
            return Ok(result);
        }
        [HttpGet("subject-income")]
        public async Task<IActionResult> GetSubjectIncome()
        {
            var result = await _chartService.GetIncomeBySubjectAsync();
            return Ok(result);
        }
    }
}
