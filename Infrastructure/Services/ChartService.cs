using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class ChartService : IChartService
    {
        private readonly IChartRepository _chartRepository;
        public ChartService(IChartRepository chartRepository)
        {
            _chartRepository = chartRepository;
        }
        public async Task<OperationResult<List<StudentSignupMonthlyDTO>>> GetStudentSignupMonthlyAsync()
        {
            return await _chartRepository.GetStudentSignupMonthlyAsync();
        }
        public async Task<OperationResult<List<RevenueByMonthDTO>>> GetRevenueByMonthAsync()
        {
            return await _chartRepository.GetRevenueByMonthAsync();
        }

        public async Task<OperationResult<List<ClassCountBySubjectDTO>>> GetClassCountBySubjectAsync()
        {
            return await _chartRepository.GetClassCountBySubjectAsync();
        }
        public async Task<OperationResult<List<ClassStatusDistributionDTO>>> GetClassStatusDistributionAsync()
        {
            return await _chartRepository.GetClassStatusDistributionAsync();
        }
        public async Task<OperationResult<List<SubjectIncomeDTO>>> GetIncomeBySubjectAsync()
        {
            return await _chartRepository.GetIncomeBySubjectAsync();
        }
    
}
}
