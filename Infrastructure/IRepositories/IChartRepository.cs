using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Infrastructure.IRepositories
{
    public interface IChartRepository
    {
        Task<OperationResult<List<StudentSignupMonthlyDTO>>> GetStudentSignupMonthlyAsync();
        Task<OperationResult<List<RevenueByMonthDTO>>> GetRevenueByMonthAsync();
        Task<OperationResult<List<ClassCountBySubjectDTO>>> GetClassCountBySubjectAsync();
        Task<OperationResult<List<ClassStatusDistributionDTO>>> GetClassStatusDistributionAsync();
        Task<OperationResult<List<SubjectIncomeDTO>>> GetIncomeBySubjectAsync();
        Task<OperationResult<List<ClassCompletionRateByMonthDTO>>> GetClassCompletionRateByMonthAsync();
    }
}
