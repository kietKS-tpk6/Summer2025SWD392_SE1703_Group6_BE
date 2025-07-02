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

        [HttpPost("mcq/import/excel")]
        public async Task<IActionResult> ImportMCQExcel([FromForm] UploadExcelRequest request)
        {
            var result = await _importExcelService.ImportMCQByExcelAsync(request.File);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("mcq/import/guide-doc")]
        public IActionResult DownloadWordGuide()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "HuongDanNhapCauHoi.docx");
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = "HuongDanNhapCauHoi.docx";

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }
    }
    public class UploadExcelRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
