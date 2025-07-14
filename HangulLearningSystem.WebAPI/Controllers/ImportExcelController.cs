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
        [HttpPost("schedule/import/excel")]
        public async Task<IActionResult> ImportScheduleExcel([FromForm] UploadExcelRequest request)
        {
            var result = await _importExcelService.ImportScheduleByExcelAsync(request.File);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("mcq/import/excel")]
        public async Task<IActionResult> ImportMCQExcel([FromForm] UploadExcelRequest request)
        {
            var result = await _importExcelService.ImportMCQByExcelAsync(request.File);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("barem/import/excel")]
        public async Task<IActionResult> ImportBaremExcel([FromForm] UploadExcelRequest request, int scoreQuestion)
        {
            var result = await _importExcelService.ImportBaremWritingByExcelAsync(request.File, scoreQuestion);
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
        [HttpGet("schedule/import/guide-doc")]
        public IActionResult DownloadScheduleGuideDoc()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "HuongDanNhapLichHoc.docx");
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = "HuongDanNhapLichHoc.docx";

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { Message = "Không tìm thấy file hướng dẫn." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }
        [HttpGet("barem/import/guide-doc")]
        public IActionResult DownloadBaremGuideDoc()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "HuongDanNhapBarem.docx");
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = "HuongDanNhapBarem.docx";
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { Message = "Không tìm thấy file hướng dẫn." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }
    }
    public class UploadExcelRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
