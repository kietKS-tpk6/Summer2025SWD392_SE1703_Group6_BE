using Application.IServices;
using Domain.Entities;
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
            var result = await _exportExcelService.ExportStudentMarkAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);

            var fileName = $"BangDiem_{classId}.xlsx";

            return File(
                fileContents: result.Data,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
        [HttpGet("export-attendance/{classId}")]
        public async Task<IActionResult> ExportAttendanceAsync(string classId)
        {
            var result = await _exportExcelService.ExportAttendanceAsync(classId);
            if (!result.Success)
                return BadRequest(result.Message);
            var fileName = $"DiemDanh_{classId}.xlsx";

            return File(
                fileContents: result.Data,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
        [HttpGet("export-schedule/{subjectId}")]
        public async Task<IActionResult> ExportScheduleAsync(string subjectId)
        {
            var result = await _exportExcelService.ExportScheduleAsync(subjectId);
            if (!result.Success)
                return BadRequest(result.Message);
            var fileName = $"LichTrinhGiangDay_{subjectId}.xlsx";

            return File(
                fileContents: result.Data,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
        [HttpGet("export-account")]
        public async Task<IActionResult> ExportAccountAsync()
        {
            var result = await _exportExcelService.ExportAccountAsync();
            if (!result.Success)
                return BadRequest(result.Message);
            var fileName = $"DanhSachTaiKhoan.xlsx";

            return File(
                fileContents: result.Data,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
    }
}
