using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportExcelController : ControllerBase
    {
        private readonly IExportExcelService _exportExcelService;
        public ExportExcelController(IExportExcelService exportExcelService)
        {
            _exportExcelService = exportExcelService;
        }
        [HttpGet("export-student-mark/{classId}")]
        public async Task<IActionResult> ExportStudentMarkTemplate(string classId)
        {
            var result = await _exportExcelService.ExportStudentMarkTemplateAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);

            var fileName = $"BangDiem_{classId}.xlsx";

            return File(
                fileContents: result.Data,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
    }
}
