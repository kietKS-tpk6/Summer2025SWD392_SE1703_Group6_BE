using Application.IServices;
using Application.Usecases.Command;
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
        [HttpPut("update-config")]
        public async Task<IActionResult> UpdateConfig([FromBody] UpdateSystemConfigCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-all-config")]
        public async Task<IActionResult> GetAllConfig()
        {
            var result = await _systemConfigService.GetListSystemConfigAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get-config-by-key/{key}")]
        public async Task<IActionResult> GetConfigByKey(string key)
        {
            var result = await _systemConfigService.GetConfig(key);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
      
    }
}
