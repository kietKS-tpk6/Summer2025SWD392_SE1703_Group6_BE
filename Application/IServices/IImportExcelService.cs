using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.IServices
{
    public  interface IImportExcelService
    {
        Task<OperationResult<List<ScheduleExcelDTO>>> ImportScheduleByExcelAsync(IFormFile file);
        Task<OperationResult<List<QuestionMCQExcelDTO>>> ImportMCQByExcelAsync(IFormFile file);
    }
}
