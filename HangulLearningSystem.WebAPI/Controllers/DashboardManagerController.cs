using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardManagerController : ControllerBase
    {
        private readonly IDashboardManagerService _dashboardManagerService;

        public DashboardManagerController(IDashboardManagerService dashboardManagerService)
        {
            _dashboardManagerService = dashboardManagerService;
        }
        [HttpGet("right-sidebar")]
        public async Task<IActionResult> GetRightSidebar()
        {
            var result = await _dashboardManagerService.GetDataForSidebarRightAsync();
            return Ok(result);
        }
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var result = await _dashboardManagerService.GetOverviewAsync();
            return Ok(result);
        }
        [HttpGet("alert-task")]
        public async Task<IActionResult> GetAlertTask()
        {
            var result = await _dashboardManagerService.GetAlertTasksAsync();
            return Ok(result);
        }
    }
}
