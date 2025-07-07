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
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IQuestionService _questionService;


        public QuestionsController (IMediator mediator, IQuestionService questionService)
        {
            _mediator = mediator;
            _questionService = questionService;
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
        [HttpPut("questions/bulk-update")]
        public async Task<IActionResult> UpdateMultipleQuestions([FromBody] UpdateMultipleQuestionsCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("mcq-option")]
        public async Task<IActionResult> DeleteMCQOption([FromBody] DeleteMCQOptionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("soft-delete/{questionId}")]
        public async Task<IActionResult> SoftDeleteQuestion(string questionId)
        {
            var result = await _mediator.Send(new SoftDeleteQuestionCommand { QuestionID = questionId });
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("by-test/{testId}")]
        public async Task<IActionResult> GetQuestionsByTestId(string testId)
        {
            var result = await _questionService.GetQuestionsByTestIdAsync(testId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
    }
}
