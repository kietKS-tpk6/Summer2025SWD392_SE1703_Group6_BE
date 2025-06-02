using Application.Usecases.Command;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Common.Constants;
using System.Threading;
using System.Threading.Tasks;
using Application.IServices;
using Infrastructure.Services;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IClassService _classService;
        public ClassController(IMediator mediator, IClassService classService)
        {
            _mediator = mediator;
            _classService = classService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ClassCreateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.CreateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail);
            }
        }
        [HttpGet("get-all-paginated")]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classService.GetListAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-subject")]
        public async Task<IActionResult> GetListBySubject([FromQuery] string subjectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classService.GetListBySubjectAsyn(subjectId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-teacher")]
        public async Task<IActionResult> GetListByTeacher([FromQuery] string teacherId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classService.GetListByTeacherAsync(teacherId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-subject-teacher")]
        public async Task<IActionResult> GetListBySubjectAndTeacher([FromQuery] string subjectId, [FromQuery] string teacherId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classService.GetListBySubjectAndTeacherAsync(subjectId, teacherId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("get-by-status")]
        public async Task<IActionResult> GetListByStatus([FromQuery] string status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classService.GetListByStatusAsync(status, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchClass([FromQuery] string keyword)
        {
            try
            {
                var result = await _classService.SearchClassAsync(keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}
