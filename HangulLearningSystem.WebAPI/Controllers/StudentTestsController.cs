using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        public class StudentTestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStudentTestService _studentTestService;
        public StudentTestsController(IMediator mediator, IStudentTestService studentTestService)
        {
            _mediator = mediator;
            _studentTestService = studentTestService;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitStudentTest([FromBody] SubmitStudentTestCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
