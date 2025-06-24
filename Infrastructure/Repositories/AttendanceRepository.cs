using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
       private readonly HangulLearningSystemDbContext _dbContext;
       public AttendanceRepository(HangulLearningSystemDbContext dbContext)
       {
            _dbContext = dbContext;
       }
        public async Task<OperationResult<string>> SetupAttendaceByClassIdAsync(
         string classId,
         List<StudentDTO> students,
         List<LessonDTO> lessons)
        {
            try
            {
                var newAttendanceRecords = new List<AttendanceRecord>();
                int currentCount = await _dbContext.AttendanceRecord.CountAsync();

                foreach (var student in students)
                {
                    foreach (var lesson in lessons)
                    {
                        var newId = "AR" + (currentCount + newAttendanceRecords.Count + 1).ToString("D6");

                        newAttendanceRecords.Add(new AttendanceRecord
                        {
                            AttendaceID = newId,
                            StudentID = student.StudentID,
                            ClassLessonID = lesson.ClassLessonID,
                            Status = AttendanceStatus.NotAvailable,
                            Note = null
                        });
                    }
                }

                await _dbContext.AttendanceRecord.AddRangeAsync(newAttendanceRecords);
                await _dbContext.SaveChangesAsync();

                return OperationResult<string>.Ok($"Đã khởi tạo {newAttendanceRecords.Count} bản ghi điểm danh cho lớp {classId}");
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Lỗi khi khởi tạo điểm danh: {ex.Message}");
            }
        }
        public async Task<OperationResult<AttendanceRecordDTO>> GetAttendanceAsync(string classId)
        {
            try
            {
                var lessons = await _dbContext.Lesson
                    .Include(l => l.SyllabusSchedule)
                    .Where(l => l.ClassID == classId)
                    .OrderBy(l => l.StartTime)
                    .ToListAsync();

                if (!lessons.Any())
                {
                    return OperationResult<AttendanceRecordDTO>.Fail(OperationMessages.NotFound("tiết học"));
                }

                var lessonIds = lessons.Select(l => l.ClassLessonID).ToList();

                var attendanceRecords = await _dbContext.AttendanceRecord
                    .Where(a => lessonIds.Contains(a.ClassLessonID))
                    .Include(a => a.Student)
                    .ToListAsync();

                var attendanceDTO = new AttendanceRecordDTO
                {
                    LessonAttendances = lessons.Select(lesson => new LessonAttendanceDTO
                    {
                        LessonID = lesson.ClassLessonID,
                        LessonTitle = lesson.SyllabusSchedule?.LessonTitle ?? "(Không tiêu đề)",
                        StudentAttendanceRecords = attendanceRecords
                            .Where(r => r.ClassLessonID == lesson.ClassLessonID)
                            .Select(r => new StudentAttendanceRecordDTO
                            {
                                AttendanceRecordID = r.AttendaceID,
                                StudentID = r.StudentID,
                                StudentName = r.Student?.Fullname ?? "(Không tên)",
                                AttendanceStatus = r.Status,
                                Note = r.Note ?? string.Empty
                            }).ToList()
                    }).ToList()
                };

                return OperationResult<AttendanceRecordDTO>.Ok(attendanceDTO, OperationMessages.RetrieveSuccess("thông tin điểm danh"));
            }
            catch (Exception ex)
            {
                return OperationResult<AttendanceRecordDTO>.Fail($"Lỗi khi truy xuất điểm danh: {ex.Message}");
            }
        }

        public async Task<OperationResult<LessonAttendanceDTO>> GetAttendanceByLessonIdAsync(string lessonId)
        {
            try
            {
                var lessonTitle = await _dbContext.Lesson
                    .Where(l => l.ClassLessonID == lessonId)
                    .Select(l => l.SyllabusSchedule.LessonTitle)
                    .FirstOrDefaultAsync();

                var records = await _dbContext.AttendanceRecord
                    .Include(a => a.Student)
                    .Where(a => a.ClassLessonID == lessonId)
                    .ToListAsync();

                var lessonDTO = new LessonAttendanceDTO
                {
                    LessonID = lessonId,
                    LessonTitle = lessonTitle ?? "(Không có tiêu đề)",
                    StudentAttendanceRecords = records.Select(r => new StudentAttendanceRecordDTO
                    {
                        AttendanceRecordID = r.AttendaceID,
                        StudentID = r.StudentID,
                        StudentName = r.Student?.Fullname ?? "(Không rõ)",
                        AttendanceStatus = r.Status,
                        Note = r.Note
                    }).ToList()
                };

                return OperationResult<LessonAttendanceDTO>.Ok(lessonDTO, OperationMessages.RetrieveSuccess("thông tin điểm danh"));
            }
            catch (Exception ex)
            {
                return OperationResult<LessonAttendanceDTO>.Fail($"Lỗi khi truy xuất điểm danh: {ex.Message}");
            }
        }




    }
}
