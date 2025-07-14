using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using OfficeOpenXml;
namespace Infrastructure.Services
{
    public class ExportExcelService: IExportExcelService
    {
        private readonly IStudentMarksService _studentMarksService;

        public ExportExcelService(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }
        public async Task<OperationResult<byte[]>> ExportStudentMarkTemplateAsync(string classId)
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
