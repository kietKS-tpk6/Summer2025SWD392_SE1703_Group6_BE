using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    internal class DashboardAnalyticsRepository : IDashboardAnalyticsRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public DashboardAnalyticsRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<PagedResult<PaymentTableRowDTO>>> GetPaginatedPaymentTableAsync(int page, int pageSize)
        {
            var paymentsQuery = _dbContext.Payment
                .Where(p => p.Status != PaymentStatus.Pending)
                .OrderByDescending(p => p.DayCreate);

            var totalCount = await paymentsQuery.CountAsync();

            var items = await (
                from p in paymentsQuery
                join a in _dbContext.Accounts on p.AccountID equals a.AccountID into accJoin
                from a in accJoin.DefaultIfEmpty()

                join c in _dbContext.Class on p.ClassID equals c.ClassID into classJoin
                from c in classJoin.DefaultIfEmpty()


                select new PaymentTableRowDTO
                {
                    PaymentID = p.PaymentID,
                    StudentName = a != null ?  a.FirstName + " " + a.LastName: "(Không rõ)",
                    ClassName = c != null ? c.ClassName : "(Không rõ)",
                    Amount = p.Total,
                    Status = p.Status.ToString(),
                    PaidAt = p.DayCreate
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<PaymentTableRowDTO>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return OperationResult<PagedResult<PaymentTableRowDTO>>.Ok(result, "Lấy danh sách thanh toán thành công.");
        }

        public async Task<OperationResult<List<LecturerStatisticsDTO>>> GetLecturerStatisticsAsync()
        {
            try
            {
                var lecturers = await (
                    from c in _dbContext.Class
                    where c.LecturerID != null
                    group c by new { c.LecturerID, c.Lecturer.FirstName } into g
                    select new LecturerStatisticsDTO
                    {
                        LecturerID = g.Key.LecturerID,
                        LecturerName = g.Key.FirstName,
                        TotalClasses = g.Count(c => c.Status != ClassStatus.Pending && c.Status != ClassStatus.Deleted),
                        OngoingClasses = g.Count(c => c.Status == ClassStatus.Ongoing),
                        CompletedClasses = g.Count(c => c.Status == ClassStatus.Completed),
                        CancelledClasses = g.Count(c => c.Status == ClassStatus.Cancelled),
                        OpenClasses = g.Count(c => c.Status == ClassStatus.Open),
                        TotalRevenue = (
                            from p in _dbContext.Payment
                            where p.ClassID != null
                                  && p.Status == PaymentStatus.Paid
                                  && g.Select(x => x.ClassID).Contains(p.ClassID)
                            select p.Total
                        ).Sum()
                    }
                ).ToListAsync();

                return OperationResult<List<LecturerStatisticsDTO>>.Ok(lecturers, "Lấy thống kê giảng viên thành công.");
            }
            catch (Exception ex)
            {
                return OperationResult<List<LecturerStatisticsDTO>>.Fail("Lỗi khi lấy dữ liệu thống kê giảng viên: " + ex.Message);
            }
        }
    }
}
