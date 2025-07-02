using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExcelController : ControllerBase
    {
        private readonly IImportExcelService _importExcelService;
        public ImportExcelController(IImportExcelService importExcelService)
        {
            _importExcelService = importExcelService;
        }
        [HttpPost("import-schedule-excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportScheduleExcel([FromForm] UploadExcelRequest request)
        {
           var result = await _importExcelService.ImportScheduleByExcelAsync(request.File);
            if(!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
    public class UploadExcelRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
