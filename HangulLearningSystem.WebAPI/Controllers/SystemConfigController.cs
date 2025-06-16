using Application.IServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISystemConfigService _systemConfigService;
        public SystemConfigController(IMediator mediator, ISystemConfigService systemConfigService)
        {
            _mediator = mediator;
            _systemConfigService = systemConfigService;
        }
        [HttpGet("get-config-by-key/{key}")]
        public async Task<IActionResult> GetConfigByKey(string key)
        {
            var result = await _systemConfigService.GetConfig(key);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
