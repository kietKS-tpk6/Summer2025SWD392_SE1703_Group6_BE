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

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ClassUpdateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] ClassUpdateStatusCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _classService.DeleteClassAsync(id);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("get-all-paginated")]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _classService.GetListAsync(page, pageSize);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-by-subject")]
        public async Task<IActionResult> GetListBySubject([FromQuery] string subjectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _classService.GetListBySubjectAsyn(subjectId, page, pageSize);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-by-teacher")]
        public async Task<IActionResult> GetListByTeacher([FromQuery] string teacherId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _classService.GetListByTeacherAsync(teacherId, page, pageSize);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-by-subject-teacher")]
        public async Task<IActionResult> GetListBySubjectAndTeacher([FromQuery] string subjectId, [FromQuery] string teacherId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _classService.GetListBySubjectAndTeacherAsync(subjectId, teacherId, page, pageSize);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-by-status")]
        public async Task<IActionResult> GetListByStatus([FromQuery] string status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _classService.GetListByStatusAsync(status, page, pageSize);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetClassById([FromQuery] string id)
        {
            var result = await _classService.GetClassDTOByIDAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchClass([FromQuery] string keyword)
        {
            var result = await _classService.SearchClassAsync(keyword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
    }

}
