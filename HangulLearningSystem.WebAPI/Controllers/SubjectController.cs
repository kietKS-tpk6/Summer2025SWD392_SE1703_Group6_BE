using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly CreateSubjectCommandHandler _createSubjectCommandHandler;
        private readonly UpdateSubjectCommandHandler _updateSubjectCommandHandler;
        private readonly DeleteSubjectCommandHandler _deleteSubjectCommandHandler;
        private readonly ISubjectService _subjectService;

        public SubjectController(
            CreateSubjectCommandHandler createSubjectCommandHandler,
            UpdateSubjectCommandHandler updateSubjectCommandHandler,
            DeleteSubjectCommandHandler deleteSubjectCommandHandler,
            ISubjectService subjectService)
        {
            _createSubjectCommandHandler = createSubjectCommandHandler;
            _updateSubjectCommandHandler = updateSubjectCommandHandler;
            _deleteSubjectCommandHandler = deleteSubjectCommandHandler;
            _subjectService = subjectService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command)
        {
            var result = await _createSubjectCommandHandler.Handle(command, CancellationToken.None);

            if (result.Contains("successfully"))
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpGet]
        public async Task<ActionResult<List<SubjectDTO>>> GetAllSubjects([FromQuery] bool? isActive = null)
        {
            var subjects = await _subjectService.GetAllSubjectsAsync(isActive);
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDTO>> GetSubjectById(string id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null)
                return NotFound(new { message = $"Subject with ID {id} not found" });

            return Ok(subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] UpdateSubjectCommand command)
        {
            if (id != command.SubjectID)
                return BadRequest(new { message = "Subject ID mismatch" });

            var result = await _updateSubjectCommandHandler.Handle(command, CancellationToken.None);

            if (result.Contains("successfully"))
                return Ok(new { message = result });

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            var command = new DeleteSubjectCommand { SubjectID = id };
            var result = await _deleteSubjectCommandHandler.Handle(command, CancellationToken.None);

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
    }
}