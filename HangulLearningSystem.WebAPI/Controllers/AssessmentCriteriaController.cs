using Application.Usecases.Command;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Common.Constants;
using System.Threading;
using System.Threading.Tasks;
using Application.IServices;
namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentCriteriaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;

        public AssessmentCriteriaController(IMediator mediator, IAssessmentCriteriaService assessmentCriteriaService)
        {
            _mediator = mediator;
            _assessmentCriteriaService = assessmentCriteriaService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AssessmentCriteriaCreateCommand command, CancellationToken cancellationToken)
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
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromBody] AssessmentCriteriaUpdateCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.UpdateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.UpdateFail);
            }
        }
        //[HttpGet("get-all-paginated")]
        //public async Task<IActionResult> GetPaginatedList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    try
        //    {
        //        var result = await _assessmentCriteriaService.GetPaginatedListAsync(page, pageSize);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
        //    }
        //}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _assessmentCriteriaService.DeleteAsync(id);

            if (success)
            {
                return Ok(OperationMessages.DeleteSuccess);
            }
            else
            {
                return NotFound(OperationMessages.DeleteFail);
            }
        }
        [HttpGet("get-by-syllabus/{syllabusId}")]
        public async Task<IActionResult> GetBySyllabusId(string syllabusId)
        {
            var result = await _assessmentCriteriaService.GetListBySyllabusIdAsync(syllabusId);

            if (result == null || !result.Any())
            {
                return NotFound(new { message = OperationMessages.NotFound });
            }

            return Ok(result);
        }
     


    }
}
