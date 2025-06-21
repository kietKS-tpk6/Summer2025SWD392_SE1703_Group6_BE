using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController (IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("generate-empty")]
        public async Task<IActionResult> CreateEmptyQuestions([FromBody] CreateQuestionsCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("questions/update")]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
