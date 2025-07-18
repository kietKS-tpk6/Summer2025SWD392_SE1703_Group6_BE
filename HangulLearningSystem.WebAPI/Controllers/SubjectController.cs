﻿using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubjectService _subjectService;

        public SubjectController(IMediator mediator, ISubjectService subjectService)
        {
            _mediator = mediator;
            _subjectService = subjectService;
        }
        [HttpPost("try-activate/{id}")]
        public async Task<IActionResult> TryActivateSubject(string id)
        {
            var result = await _subjectService.TryActivateSubjectAsync(id);

            if (result.Contains("successfully") || result.Contains("already active"))
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command)
        {
            var result = await _mediator.Send(command);

            if (result?.Success == true)
            {
                return Ok(new { message = result });
            }
            else
            {
                return BadRequest(new { message = result });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSubject([FromBody] UpdateSubjectCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Contains("successfully"))
                return Ok(new { message = result });

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateSubjectStatus([FromBody] UpdateSubjectStatusCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Contains("successfully"))
                return Ok(new { message = result });

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            var command = new DeleteSubjectCommand { SubjectID = id };
            var result = await _mediator.Send(command);

            if (result.Contains("successfully"))
                return Ok(new { message = result });

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetSubjectsCount()
        {
            var count = await _subjectService.GetTotalSubjectsCountAsync();
            return Ok(count);
        }

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> CheckSubjectExists(string id)
        {
            var exists = await _subjectService.SubjectExistsAsync(id);
            return Ok(exists);
        }

        [HttpGet("get-by-status")]
        public async Task<IActionResult> GetSubjectsByStatus([FromQuery] SubjectStatus status)
        {
            var result = await _subjectService.GetSubjectByStatusAsync(status);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<SubjectDTO>>> GetAllSubjects([FromQuery] SubjectStatus? status = null)
        {
            SubjectStatus? filterStatus = status;
            var subjects = await _subjectService.GetAllSubjectsAsync();

            // Filter by status if provided
            if (filterStatus.HasValue)
            {
                subjects = subjects.Where(s => s.Status == filterStatus.Value).ToList();
            }

            return Ok(subjects);
        }

        [HttpGet("get-subject-by-{id}")]
        public async Task<ActionResult<SubjectDTO>> GetSubjectById(string id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null)
                return NotFound(new { message = $"Subject with ID {id} not found" });

            return Ok(subject);
        }
    }
}