using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly CreateSyllabusesCommandHandler _createSyllabusesCommandHandler;

        public SyllabusController(CreateSyllabusesCommandHandler createSyllabusesCommandHandler)
        {
            _createSyllabusesCommandHandler = createSyllabusesCommandHandler;
        }

        [HttpPost("create-syllabus")]
        public async Task<IActionResult> CreateSyllabus([FromBody] CreateSyllabusesCommand command)
        {
            var result = await _createSyllabusesCommandHandler.Handle(command, CancellationToken.None);
            if (result == null)
            {
                return BadRequest("Tạo chương trình học thất bại");
            }
            return Ok(result);
        }
    }
}
