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
    }
}
