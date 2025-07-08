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
        public async Task<OperationResult<List<RevenueByMonthDTO>>> GetRevenueByMonthAsync()
        {
            var today = DateTime.Today;
            var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5);

            var rawData = await _dbContext.Payment
                .Where(p => p.DayCreate >= startMonth && p.Status == PaymentStatus.Paid)
                .GroupBy(p => new { p.DayCreate.Year, p.DayCreate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => x.Total)
                })
                .ToListAsync();

            var monthList = Enumerable.Range(0, 6)
                .Select(i => startMonth.AddMonths(i))
                .ToList();

            var result = monthList.Select(month =>
            {
                var data = rawData.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);
                return new RevenueByMonthDTO
                {
                    Month = $"{month.Month:D2}/{month.Year}",
                    Revenue = data?.Revenue ?? 0
                };
            }).ToList();

            return OperationResult<List<RevenueByMonthDTO>>.Ok(result, OperationMessages.RetrieveSuccess("doanh thu theo tháng"));
        }
        public async Task<OperationResult<List<ClassCountBySubjectDTO>>> GetClassCountBySubjectAsync()
        {
            var data = await _dbContext.Class
                .Where(c => c.Status == ClassStatus.Open || c.Status == ClassStatus.Ongoing || c.Status == ClassStatus.Completed)
                .GroupBy(c => c.Subject.SubjectName)
                .Select(g => new ClassCountBySubjectDTO
                {
                    Subject = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return OperationResult<List<ClassCountBySubjectDTO>>.Ok(data, OperationMessages.RetrieveSuccess("số lượng lớp theo môn học"));
        }
        public async Task<OperationResult<List<ClassStatusDistributionDTO>>> GetClassStatusDistributionAsync()
        {
            var data = await _dbContext.Class
                .GroupBy(c => c.Status)
                .Select(g => new ClassStatusDistributionDTO
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return OperationResult<List<ClassStatusDistributionDTO>>.Ok(
                data,
                OperationMessages.RetrieveSuccess("phân bố trạng thái lớp học")
            );
        }
        public async Task<OperationResult<List<SubjectIncomeDTO>>> GetIncomeBySubjectAsync()
        {
            var data = await (
                from payment in _dbContext.Payment
                where payment.Status == PaymentStatus.Paid
                join cls in _dbContext.Class on payment.ClassID equals cls.ClassID
                join subject in _dbContext.Subject on cls.SubjectID equals subject.SubjectID
                group payment by subject.SubjectName into g
                select new SubjectIncomeDTO
                {
                    SubjectName = g.Key,
                    TotalRevenue = g.Sum(p => p.Total)
                }
            ).ToListAsync();

            return OperationResult<List<SubjectIncomeDTO>>.Ok(
                data,
                OperationMessages.RetrieveSuccess("doanh thu theo môn học")
            );
        }
    }
}
