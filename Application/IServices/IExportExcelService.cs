using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;

namespace Application.IServices
{
    public interface IExportExcelService
    {
        Task<OperationResult<byte[]>> ExportStudentMarkAsync(string classId);
        Task<OperationResult<byte[]>> ExportAttendanceAsync(string classId);
    }
}
