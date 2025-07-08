using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChartRepository : IChartRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public ChartRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<List<StudentSignupMonthlyDTO>>> GetStudentSignupMonthlyAsync()
        {
            var today = DateTime.Today;
            var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5); 

            var rawData = await _dbContext.ClassEnrollment
                .Where(e => e.EnrolledDate >= startMonth && e.Status == EnrollmentStatus.Actived)
                .GroupBy(e => new { e.EnrolledDate.Year, e.EnrolledDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            var monthList = Enumerable.Range(0, 6)
                .Select(i => startMonth.AddMonths(i))
                .ToList();

            var result = monthList.Select(month =>
            {
                var data = rawData.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);
                return new StudentSignupMonthlyDTO
                {
                    Month = $"{month.Month:D2}/{month.Year}",
                    Count = data?.Count ?? 0
                };
            }).ToList();

            return OperationResult<List<StudentSignupMonthlyDTO>>.Ok(result, OperationMessages.RetrieveSuccess("số lượt học viên đăng ký lớp học"));
        }


    }
}
