using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Infrastructure.Services
{
    public class ImportExcelService : IImportExcelService
    {
        private readonly ISystemConfigService _systemConfigService;
        public ImportExcelService(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }
        public async Task<OperationResult<List<ScheduleExcelDTO>>> ImportScheduleByExcelAsync(IFormFile file)
        {
            var maxDuration = await _systemConfigService.GetConfig("max_total_minutes_allowed");
            if(!maxDuration.Success || maxDuration.Data == null)
            {
                return OperationResult<List<ScheduleExcelDTO>>.Fail(OperationMessages.NotFound("cấu hình"));
            }

            if (file == null || file.Length == 0)
                return OperationResult<List<ScheduleExcelDTO>>.Fail("File Excel không hợp lệ.");

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return OperationResult<List<ScheduleExcelDTO>>.Fail("Không tìm thấy sheet trong file Excel.");

                var list = new List<ScheduleExcelDTO>();
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var dto = new ScheduleExcelDTO();

                    if (!int.TryParse(worksheet.Cells[row, 1].Text.Trim(), out int week))
                        return OperationResult<List<ScheduleExcelDTO>>.Fail($"Tuần không hợp lệ tại dòng {row}.");

                    if (!int.TryParse(worksheet.Cells[row, 2].Text.Trim(), out int slot))
                        return OperationResult<List<ScheduleExcelDTO>>.Fail($"Tiết không hợp lệ tại dòng {row}.");

                    var title = worksheet.Cells[row, 3].Text.Trim();
                    var content = worksheet.Cells[row, 4].Text.Trim();
                    if (!int.TryParse(worksheet.Cells[row, 5].Text.Trim(), out int duration))
                        return OperationResult<List<ScheduleExcelDTO>>.Fail($"Thời lượng không hợp lệ tại dòng {row}.");
                    int maxDurationValue = Convert.ToInt32(maxDuration.Data.Value);

                    if (duration <= 0 || duration > maxDurationValue)
                        return  OperationResult<List<ScheduleExcelDTO>>.Fail($"Thời lượng phải trong khoảng 1-{maxDuration.Data.Value} phút tại dòng {row}.");

                    var resource = worksheet.Cells[row, 6].Text.Trim();

                    dto.Week = week;
                    dto.Slot = slot;
                    dto.Title = title;
                    dto.Content = content;
                    dto.DurationMinutes = duration;
                    dto.ResourceUrl = resource;

                    list.Add(dto);
                }

                return OperationResult<List<ScheduleExcelDTO>>.Ok(list, "Nhập thời khóa biểu từ Excel thành công.");
            }
            catch (Exception ex)
            {
                return OperationResult<List<ScheduleExcelDTO>>.Fail("Đã xảy ra lỗi khi đọc file Excel: " + ex.Message);
            }
        }

    }
}
