using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.IServices
{
    public interface IChartService
    {
        Task<OperationResult<List<StudentSignupMonthlyDTO>>> GetStudentSignupMonthlyAsync();
        Task<OperationResult<List<RevenueByMonthDTO>>> GetRevenueByMonthAsync();
        Task<OperationResult<List<ClassCountBySubjectDTO>>> GetClassCountBySubjectAsync();
        Task<OperationResult<List<ClassStatusDistributionDTO>>> GetClassStatusDistributionAsync();
        Task<OperationResult<List<SubjectIncomeDTO>>> GetIncomeBySubjectAsync();
    }
}
