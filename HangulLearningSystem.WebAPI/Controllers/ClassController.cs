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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = await _classService.GetAllClassesForPaymentAsync();
                return Ok(classes);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{classId}")]
        public async Task<IActionResult> GetClassDetail(string classId)
        {
            try
            {
                var classDetail = await _classService.GetClassDetailForPaymentAsync(classId);

                if (classDetail == null)
                {
                    return NotFound(new { message = "Class not found" });
                }

                return Ok(classDetail);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{classId}/availability")]
        public async Task<IActionResult> CheckClassAvailability(string classId)
        {
            try
            {
                var isAvailable = await _classService.IsClassAvailableForEnrollmentAsync(classId);
                var availableSlots = await _classService.GetAvailableSlotsAsync(classId);

                return Ok(new
                {
                    isAvailable,
                    availableSlots
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
