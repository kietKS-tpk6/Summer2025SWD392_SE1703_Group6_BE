using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Domain.Enums;
using OfficeOpenXml;
namespace Infrastructure.Services
{
    public class ExportExcelService: IExportExcelService
    {
        private readonly IStudentMarksService _studentMarksService;
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly IAttendanceService _attendanceService;
        private readonly IAccountService _accountService;
        private readonly IPaymentService _paymentService;

        public ExportExcelService
            (
            IStudentMarksService studentMarksService, 
            IAttendanceService attendanceService,
            ISyllabusScheduleService syllabusScheduleService,
            IAccountService accountService,
            IPaymentService paymentService
            )
        {
            _studentMarksService = studentMarksService;
            _attendanceService = attendanceService;
            _syllabusScheduleService = syllabusScheduleService;
            _accountService = accountService;
            _paymentService = paymentService;
        }
        public async Task<OperationResult<byte[]>> ExportStudentMarkAsync(string classId)
        {
            var studentMarkResult = await _studentMarksService.GetStudentMarkDetailDTOByClassIdAsync(classId);
            if (!studentMarkResult.Success)
                return OperationResult<byte[]>.Fail(studentMarkResult.Message);

            var markGroups = studentMarkResult.Data;

            ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("BangDiem");

            ws.Cells[1, 1].Value = "STT";
            ws.Cells[1, 2].Value = "Tên học sinh";

            var criteriaHeaders = new List<(string Header, double? Weight, int ColumnIndex)>();
            int col = 3;

            foreach (var group in markGroups)
            {
                string categoryName = group.AssessmentCategory.ToString();
                string header = group.AssessmentIndex > 1 ? $"{categoryName} {group.AssessmentIndex}" : categoryName;
                string headerWithWeight = $"{header} ({group.WeightPercent:0.##}%)";

                criteriaHeaders.Add((headerWithWeight, group.WeightPercent, col));
                ws.Cells[1, col].Value = headerWithWeight;
                col++;
            }

            ws.Cells[1, col].Value = "Trung bình";
            int totalColCount = col;

            var allStudents = markGroups
                .SelectMany(g => g.StudentMarks)
                .GroupBy(sm => sm.StudentName)
                .Select((g, index) => new
                {
                    STT = index + 1,
                    Name = g.Key
                })
                .ToList();

            int row = 2;
            foreach (var student in allStudents)
            {
                ws.Cells[row, 1].Value = student.STT;
                ws.Cells[row, 2].Value = student.Name;

                for (int i = 0; i < criteriaHeaders.Count; i++)
                {
                    var group = markGroups[i];
                    var columnIndex = criteriaHeaders[i].ColumnIndex;
                    var mark = group.StudentMarks.FirstOrDefault(s => s.StudentName == student.Name)?.Mark;
                    ws.Cells[row, columnIndex].Value = mark;
                }

                var formulaParts = criteriaHeaders.Select(ch =>
                {
                    string colLetter = GetColumnLetter(ch.ColumnIndex);
                    return $"{colLetter}{row}*{(ch.Weight ?? 0)}/100";
                });

                ws.Cells[row, totalColCount].Formula = string.Join("+", formulaParts);
                row++;
            }

            for (int c = 1; c <= totalColCount; c++)
            {
                ws.Column(c).AutoFit();
                ws.Cells[1, c].Style.Font.Bold = true;
            }

            return OperationResult<byte[]>.Ok(await package.GetAsByteArrayAsync(), "Xuất file Excel thành công.");
        }
        public async Task<OperationResult<byte[]>> ExportAttendanceAsync(string classId)
        {
            var attendanceResult = await _attendanceService.GetAttendanceAsync(classId);
            if (!attendanceResult.Success)
                return OperationResult<byte[]>.Fail(attendanceResult.Message);

            var lessons = attendanceResult.Data.LessonAttendances;

            ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("TinhTrangDiemDanh");

            ws.Cells[1, 1].Value = "STT";
            ws.Cells[1, 2].Value = "Tên học viên";
            for (int i = 0; i < lessons.Count; i++)
            {
                ws.Cells[1, i + 3].Value = $"Slot {i + 1}";
            }
            ws.Cells[1, lessons.Count + 3].Value = "Tổng buổi có mặt";

            var allStudents = lessons
                .SelectMany(l => l.StudentAttendanceRecords)
                .GroupBy(s => s.StudentID)
                .Select(g => new
                {
                    StudentID = g.Key,
                    StudentName = g.First().StudentName
                })
                .ToList();

            int row = 2;
            foreach (var student in allStudents)
            {
                ws.Cells[row, 1].Value = row - 1; 
                ws.Cells[row, 2].Value = student.StudentName;

                int presentCount = 0;

                for (int i = 0; i < lessons.Count; i++)
                {
                    var lesson = lessons[i];
                    var record = lesson.StudentAttendanceRecords
                        .FirstOrDefault(s => s.StudentID == student.StudentID);

                    var status = (AttendanceStatus?)record?.AttendanceStatus;
                    var cell = ws.Cells[row, i + 3];
                    cell.Value = status?.ToString();

                    if (status == AttendanceStatus.Present)
                    {
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        presentCount++;
                    }
                    else if (status == AttendanceStatus.Absence)
                    {
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
                    }
                    else if (status == AttendanceStatus.NotAvailable)
                    {
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                    }
                }

                ws.Cells[row, lessons.Count + 3].Value = presentCount;
                row++;
            }

            for (int col = 1; col <= lessons.Count + 3; col++)
            {
                ws.Column(col).AutoFit();
                ws.Cells[1, col].Style.Font.Bold = true;
            }

            return OperationResult<byte[]>.Ok(await package.GetAsByteArrayAsync(), "Xuất file điểm danh thành công.");
        }

        public async Task<OperationResult<byte[]>> ExportScheduleAsync(string subjectId)
        {
            var scheduleResult = await _syllabusScheduleService.GetScheduleBySubjectIdAsync(subjectId);
            if (!scheduleResult.Success)
                return OperationResult<byte[]>.Fail(scheduleResult.Message);

            var schedules = scheduleResult.Data
                .OrderBy(s => s.Week)
                .ThenBy(s => s.Slot)
                .ToList();

            ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("LichTrinhGiangDay");

            ws.Cells[1, 1].Value = "Tuần";
            ws.Cells[1, 2].Value = "Slot";
            ws.Cells[1, 3].Value = "Tiêu đề bài học";
            ws.Cells[1, 4].Value = "Nội dung";
            ws.Cells[1, 5].Value = "Thời lượng";
            ws.Cells[1, 6].Value = "Tài nguyên";

            int row = 2;
            int slot = 1;
            foreach (var s in schedules)
            {
                ws.Cells[row, 1].Value = s.Week;
                ws.Cells[row, 2].Value = slot;
                ws.Cells[row, 3].Value = s.LessonTitle;
                ws.Cells[row, 4].Value = s.Content;
                ws.Cells[row, 5].Value = $"{s.DurationMinutes} phút";
                ws.Cells[row, 6].Value = s.Resources;
                row++;
                slot++;
            }

            int startRow = 2;
            int endRow = 2;
            int totalRows = schedules.Count;

            for (int i = 1; i < totalRows; i++)
            {
                bool isSameWeek = schedules[i].Week == schedules[i - 1].Week;

                if (isSameWeek)
                {
                    endRow++;
                }
                else
                {
                    if (startRow != endRow)
                        ws.Cells[startRow, 1, endRow, 1].Merge = true;

                    startRow = endRow = startRow + (endRow - startRow) + 1;
                }
            }

            if (startRow != endRow)
            {
                ws.Cells[startRow, 1, endRow, 1].Merge = true;
            }

            for (int col = 1; col <= 6; col++)
            {
                ws.Column(col).AutoFit();
                ws.Cells[1, col].Style.Font.Bold = true;
            }

            return OperationResult<byte[]>.Ok(await package.GetAsByteArrayAsync(), "Xuất thời khóa biểu thành công.");
        }
        public async Task<OperationResult<byte[]>> ExportAccountAsync()
        {
            var accountResult = await _accountService.GetAllAccountForExcelAsync();
            if (!accountResult.Success)
                return OperationResult<byte[]>.Fail(accountResult.Message);

            var accounts = accountResult.Data;

            ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("TaiKhoan");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Họ Tên";
            ws.Cells[1, 3].Value = "Email";
            ws.Cells[1, 4].Value = "Số điện thoại";
            ws.Cells[1, 5].Value = "Vai trò";
            ws.Cells[1, 6].Value = "Trạng thái";

            int row = 2;
            foreach (var a in accounts)
            {
                ws.Cells[row, 1].Value = a.AccountID;
                ws.Cells[row, 2].Value = $"{a.LastName} {a.FirstName}";
                ws.Cells[row, 3].Value = a.Email;
                ws.Cells[row, 4].Value = a.PhoneNumber;
                ws.Cells[row, 5].Value = a.Role?.ToString();     
                ws.Cells[row, 6].Value = a.Status?.ToString();   
                row++;
            }

            for (int col = 1; col <= 6; col++)
            {
                ws.Column(col).AutoFit();
                ws.Cells[1, col].Style.Font.Bold = true;
            }

            return OperationResult<byte[]>.Ok(await package.GetAsByteArrayAsync(), "Xuất danh sách tài khoản thành công.");
        }
        public async Task<OperationResult<byte[]>> ExportPaymentAsync()
        {
            var paymentResult = await _paymentService.GetPaymentForExcelAsync();
            if (!paymentResult.Success)
                return OperationResult<byte[]>.Fail(paymentResult.Message);
            var payments = paymentResult.Data;
            ExcelPackage.License.SetNonCommercialPersonal("HangulLearningSystem");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("ThanhToan");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Tên học viên";
            ws.Cells[1, 3].Value = "Lớp học";
            ws.Cells[1, 4].Value = "Số tiền";
            ws.Cells[1, 5].Value = "Trạng thai";
            ws.Cells[1, 6].Value = "Ngày thanh toán";

            int row = 2;
            foreach (var p in payments)
            {
                ws.Cells[row, 1].Value = p.PaymentID;
                ws.Cells[row, 2].Value = p.StudentName;
                ws.Cells[row, 3].Value = p.ClassName;
                ws.Cells[row, 4].Value = p.Amount;
                ws.Cells[row, 5].Value = p.Status?.ToString();
                ws.Cells[row, 6].Value = p.PaidAt.ToString("dd/MM/yyyy HH:mm"); ;
                row++;
            }
            for (int col = 1; col <= 6; col++)
            {
                ws.Column(col).AutoFit();
                ws.Cells[1, col].Style.Font.Bold = true;
            }

            return OperationResult<byte[]>.Ok(await package.GetAsByteArrayAsync(), "Xuất danh sách hóa đơn thanh toán thành công.");

        }

        private string GetColumnLetter(int columnNumber)
        {
            var dividend = columnNumber;
            var columnName = "";

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

    }
}
