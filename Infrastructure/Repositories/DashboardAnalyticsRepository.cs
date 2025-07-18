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
                    StudentID = p.AccountID,
                    ClassID = p.ClassID,
                    StudentName = a != null ?  a.FirstName + " " + a.LastName : "(Không rõ)",
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

        public async Task<OperationResult<List<ClassCompletionStatsDTO>>> GetClassCompletionStatisticsAsync()
        {
            try
            {
                var completedClasses = await (from cls in _dbContext.Class
                                              join subj in _dbContext.Subject on cls.SubjectID equals subj.SubjectID
                                              where cls.Status == ClassStatus.Completed
                                              select new
                                              {
                                                  cls.ClassID,
                                                  cls.ClassName,
                                                  subj.SubjectName
                                              }).ToListAsync();

                var enrollments = await _dbContext.ClassEnrollment.ToListAsync();
                var marks = await _dbContext.StudentMarks.ToListAsync();
                var attendance = await _dbContext.AttendanceRecord.ToListAsync();
                var lessons = await _dbContext.Lesson.ToListAsync();

                var stats = completedClasses.Select(cls =>
                {
                    var classEnrollments = enrollments.Where(e => e.ClassID == cls.ClassID).ToList();
                    var totalStudents = classEnrollments.Count;
                    var completedStudents = classEnrollments.Count(e => e.Status == EnrollmentStatus.Passed);

                    var classLessons = lessons.Where(l => l.ClassID == cls.ClassID).Select(l => l.ClassLessonID).ToList();
                    var classAttendance = attendance.Where(a => classLessons.Contains(a.ClassLessonID)).ToList();

                    var groupedAttendance = classAttendance
                        .GroupBy(a => a.StudentID)
                        .Select(g =>
                        {
                            var total = g.Count();
                            var present = g.Count(a => a.Status == AttendanceStatus.Present);
                            return total > 0 ? (100.0 * present / total) : 0;
                        });

                    var avgAttendance = groupedAttendance.Any() ? groupedAttendance.Average() : 0;

                    var classMarks = marks.Where(m => m.ClassID == cls.ClassID).Select(m => (double?)m.Mark).ToList();
                    var avgScore = classMarks.Any() ? classMarks.Average() ?? 0 : 0;

                    var completionRate = totalStudents > 0 ? 100.0 * completedStudents / totalStudents : 0;

                    return new ClassCompletionStatsDTO
                    {
                        ClassId = cls.ClassID,
                        ClassName = cls.ClassName,
                        SubjectName = cls.SubjectName,
                        TotalStudents = totalStudents,
                        CompletedStudents = completedStudents,
                        AverageAttendanceRate = avgAttendance,
                        AverageScore = avgScore,
                        CompletionRate = completionRate
                    };
                }).ToList();

                return OperationResult<List<ClassCompletionStatsDTO>>.Ok(stats, "Lấy thống kê học phần thành công.");
            }
            catch (Exception ex)
            {
                return OperationResult<List<ClassCompletionStatsDTO>>.Fail("Thống kê thất bại: " + ex.Message);
            }
        }
        public async Task<OperationResult<List<StudentPerformanceInClassDTO>>> GetStudentPerformanceInClassAsync(string classId)
        {
            try
            {
                var students = await (
                    from enroll in _dbContext.ClassEnrollment
                    join acc in _dbContext.Accounts on enroll.StudentID equals acc.AccountID
                    where enroll.ClassID == classId
                    select new StudentPerformanceInClassDTO
                    {
                        StudentId = enroll.StudentID,
                        StudentName = acc.FirstName + " " + acc.LastName,

                        AttendanceRate = (
                            from a in _dbContext.AttendanceRecord
                            join l in _dbContext.Lesson on a.ClassLessonID equals l.ClassLessonID
                            where l.ClassID == classId && a.StudentID == enroll.StudentID
                            group a by a.StudentID into g
                            select 100.0 * g.Count(x => x.Status == AttendanceStatus.Present) / g.Count()
                        ).FirstOrDefault(),

                        AverageScore = _dbContext.StudentMarks
                            .Where(m => m.ClassID == classId && m.AccountID == enroll.StudentID)
                            .Average(m => (double?)m.Mark) ?? 0,

                        Status = enroll.Status == EnrollmentStatus.Passed ? "Hoàn thành" : "Chưa hoàn thành",

                        AbsentSessions = (
                            from a in _dbContext.AttendanceRecord
                            join l in _dbContext.Lesson on a.ClassLessonID equals l.ClassLessonID
                            where l.ClassID == classId && a.StudentID == enroll.StudentID
                            && a.Status != AttendanceStatus.Present
                            select a
                        ).Count()
                    }
                ).ToListAsync();

                return OperationResult<List<StudentPerformanceInClassDTO>>.Ok(students, "Lấy danh sách học viên thành công.");
            }
            catch (Exception ex)
            {
                return OperationResult<List<StudentPerformanceInClassDTO>>.Fail("Thống kê thất bại: " + ex.Message);
            }
        }
    }
}
