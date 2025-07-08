using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DashboardManagerRepository : IDashboardManagerRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;
        public DashboardManagerRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<ManagerSidebarRightDTO>> GetDataForSidebarRightAsync()
        {
            var today = DateTime.Today;

            var todayLessonsCount = await _dbContext.Lesson
                .Where(l => l.StartTime == today && l.IsActive)
                .Select(l => l.ClassID)
                .Distinct()
                .CountAsync();

            var todayTestsCount = await _dbContext.TestEvent
                .Where(t => t.StartAt.HasValue && t.StartAt.Value.Date == today)
                .CountAsync();

            // Lớp đủ điều kiện mở: trạng thái Open, ngày mở <= hôm nay, số lượng học viên >= MinStudentAcp
            var eligibleClasses = await (
                from c in _dbContext.Class
                where c.Status == ClassStatus.Open && c.TeachingStartTime <= today
                join ce in _dbContext.ClassEnrollment on c.ClassID equals ce.ClassID into ceGroup
                let studentCount = ceGroup.Count()
                where studentCount >= c.MinStudentAcp
                select new EligibleClassForOpeningInfo
                {
                    ClassID = c.ClassID,
                    ClassName = c.ClassName,
                    TeachingStartTime = c.TeachingStartTime,
                    StudentCount = studentCount
                }).ToListAsync();

            // Lớp gần tới hạn dạy (<= 7 ngày nữa), chưa tuyển đủ học viên
            var upcomingNotEligible = await (
                from c in _dbContext.Class
                where c.Status == ClassStatus.Open &&
                      c.TeachingStartTime > today &&
                      c.TeachingStartTime <= today.AddDays(7)
                join ce in _dbContext.ClassEnrollment on c.ClassID equals ce.ClassID into ceGroup
                let studentCount = ceGroup.Count()
                where studentCount < c.MinStudentAcp
                select new ClassNearOpenButNotReadyDTO
                {
                    ClassID = c.ClassID,
                    ClassName = c.ClassName,
                    TeachingStartTime = c.TeachingStartTime,
                    StudentCount = studentCount,
                    MinStudentAcpt = c.MinStudentAcp
                }).ToListAsync();

            // Các test event Final vs Midterm  trạng  thái Draft
            var testsWithoutTestID = await (
             from e in _dbContext.TestEvent
             join l in _dbContext.Lesson on e.ClassLessonID equals l.ClassLessonID
             join c in _dbContext.Class on l.ClassID equals c.ClassID
             join s in _dbContext.Subject on c.SubjectID equals s.SubjectID
             join ss in _dbContext.SyllabusSchedule on l.SyllabusScheduleID equals ss.SyllabusScheduleID
             join sst in _dbContext.SyllabusScheduleTests on ss.SyllabusScheduleID equals sst.SyllabusScheduleID
             join ac in _dbContext.AssessmentCriteria on sst.AssessmentCriteriaID equals ac.AssessmentCriteriaID
             where e.Status == TestEventStatus.Draft
                   && (ac.Category == AssessmentCategory.Final || ac.Category == AssessmentCategory.Midterm)
                   && c.Status == ClassStatus.Ongoing
                   && !_dbContext.SyllabusScheduleTests
                        .Where(sst2 => sst2.SyllabusScheduleID == ss.SyllabusScheduleID)
                        .Join(_dbContext.AssessmentCriteria,
                              sst2 => sst2.AssessmentCriteriaID,
                              ac2 => ac2.AssessmentCriteriaID,
                              (sst2, ac2) => ac2.Category)
                        .Any(cat => cat != AssessmentCategory.Final && cat != AssessmentCategory.Midterm)
             select new TestEventMissingTestIdInfo
             {
                 TestEventID = e.TestEventID,
                 SubjectName = s.SubjectName,
                 TimeLessonStart = l.StartTime,
                 Category = (AssessmentCategory)ac.Category!
             }).Distinct().ToListAsync();


            // Tổng hợp
            var result = new ManagerSidebarRightDTO
            {
                TodayClasses = todayLessonsCount,
                TodayTests = todayTestsCount,
                EligibleClassForOpening = eligibleClasses,
                ClassNearOpenButNotReady = upcomingNotEligible,
                TestEventsNeedingTestID = testsWithoutTestID
            };

            return OperationResult<ManagerSidebarRightDTO>.Ok(result, OperationMessages.RetrieveSuccess("dữ liệu sidebar phải của Manager"));
        }

        public async Task<OperationResult<ManagerDashboardOverviewDTO>> GetOverviewAsync()
        {
            var totalLecturers = await _dbContext.Accounts
                .CountAsync(a => a.Role == AccountRole.Lecture);

            var totalSubjects = await _dbContext.Subject.CountAsync(s => s.Status == SubjectStatus.Active);

            var activeClasses = await _dbContext.Class
                .CountAsync(c => c.Status == ClassStatus.Ongoing);

            var totalRevenue = await _dbContext.Payment
                .Where(p => p.Status == PaymentStatus.Paid)
                .SumAsync(p => (decimal?)p.Total) ?? 0;

            var dto = new ManagerDashboardOverviewDTO
            {
                TotalLecturers = totalLecturers,
                TotalSubjects = totalSubjects,
                ActiveClasses = activeClasses,
                TotalRevenue = totalRevenue
            };

            return OperationResult<ManagerDashboardOverviewDTO>.Ok(dto, OperationMessages.RetrieveSuccess("dữ liệu tổng quan dashboard"));
        }
        public async Task<OperationResult<List<ManagerAlertTaskDTO>>> GetAlertTasksAsync()
        {
            var today = DateTime.Today;

            var alerts = new List<ManagerAlertTaskDTO>();

            var classReadyToApprove = await (
                from c in _dbContext.Class
                where c.Status == ClassStatus.Pending &&
                      c.TeachingStartTime <= today &&
                      _dbContext.ClassEnrollment.Count(e => e.ClassID == c.ClassID) >= c.MinStudentAcp
                select new ManagerAlertTaskDTO
                {
                    Type = "class_approval",
                    Message = $"📌 Lớp '{c.ClassName}' đủ điều kiện mở, cần được duyệt.",
                    Deadline = c.TeachingStartTime,
                    Severity = "warning"
                }).ToListAsync();
            alerts.AddRange(classReadyToApprove);

            var lowEnrollment = await (
                from c in _dbContext.Class
                where c.Status == ClassStatus.Pending &&
                      c.TeachingStartTime > today &&
                      c.TeachingStartTime <= today.AddDays(5)
                let studentCount = _dbContext.ClassEnrollment.Count(e => e.ClassID == c.ClassID)
                where studentCount < c.MinStudentAcp
                select new ManagerAlertTaskDTO
                {
                    Type = "low_enrollment",
                    Message = $"⚠️ Lớp '{c.ClassName}' sắp tới ngày khai giảng nhưng chưa đủ học viên.",
                    Deadline = c.TeachingStartTime,
                    Severity = "urgent"
                }).ToListAsync();
            alerts.AddRange(lowEnrollment);

            var testMissingInfo = await (
           from e in _dbContext.TestEvent
           join lesson in _dbContext.Lesson on e.ClassLessonID equals lesson.ClassLessonID
           join cls in _dbContext.Class on lesson.ClassID equals cls.ClassID
           join schedule in _dbContext.SyllabusSchedule on lesson.SyllabusScheduleID equals schedule.SyllabusScheduleID
           join scheduletest in _dbContext.SyllabusScheduleTests on schedule.SyllabusScheduleID equals scheduletest.SyllabusScheduleID
           join assessment in _dbContext.AssessmentCriteria on scheduletest.AssessmentCriteriaID equals assessment.AssessmentCriteriaID
           where e.Status == TestEventStatus.Draft &&
                 (assessment.Category == AssessmentCategory.Midterm || assessment.Category == AssessmentCategory.Final) &&
                 cls.Status == ClassStatus.Ongoing
           select new ManagerAlertTaskDTO
           {
               Type = "test_event_missing",
               Message = $"📝 Đề kiểm tra '{assessment.Category}' của lớp '{cls.ClassName}' cần cập nhật thông tin.",
               Deadline = lesson.StartTime.AddDays(-3),
               Severity = "info"
           })
           .Distinct()
           .OrderBy(x => x.Deadline) 
           .ToListAsync();

            alerts.AddRange(testMissingInfo);


            var testNotReviewedCount = await _dbContext.Test
                .CountAsync(t => t.Status == TestStatus.Pending);

            if (testNotReviewedCount > 0)
            {
                alerts.Add(new ManagerAlertTaskDTO
                {
                    Type = "test_not_reviewed",
                    Message = $"🧐 Có {testNotReviewedCount} đề kiểm tra đang chờ được phê duyệt.",
                    Deadline = null,
                    Severity = "warning"
                });
            }

            var pendingRefundCount = await _dbContext.Payment
                .CountAsync(p => p.Status == PaymentStatus.RequestRefund);

            if (pendingRefundCount > 0)
            {
                alerts.Add(new ManagerAlertTaskDTO
                {
                    Type = "payment_pending",
                    Message = $"💰 Có {pendingRefundCount} giao dịch thanh toán đang chờ xử lý hoàn tiền.",
                    Deadline = null,
                    Severity = "info"
                });
            }

            return OperationResult<List<ManagerAlertTaskDTO>>.Ok(alerts, OperationMessages.RetrieveSuccess("danh sách cảnh báo / hành động"));
        }
    }
}
