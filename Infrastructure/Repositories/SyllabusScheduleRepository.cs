using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Application.DTOs;
using Application.Usecases.Command;
using Application.Common.Constants;
namespace Infrastructure.Repositories
{
    public class SyllabusScheduleRepository : ISyllabusScheduleRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusScheduleRepository(HangulLearningSystemDbContext context)
        {
            _dbContext = context;
        }

        public async Task<OperationResult<List<int>>> GetWeeksBySubjectIdAsync(string subjectId)
        {
            try
            {
                var weeks = await _dbContext.SyllabusSchedule
                    .Where(s => s.SubjectID == subjectId)
                    .Select(s => s.Week ?? 0 )
                    .ToListAsync();

                return OperationResult<List<int>>.Ok(weeks, OperationMessages.RetrieveSuccess("tuần học"));
            }
            catch (Exception ex)
            {
                return OperationResult<List<int>>.Fail($"Lỗi khi truy xuất tuần học: {ex.Message}");
            }
        }


        public async Task<bool> CreateMultipleSyllabusesScheduleAsync(List<SyllabusSchedule> syllabusSchedules)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Thêm tất cả SyllabusSchedule vào context
                await _dbContext.SyllabusSchedule.AddRangeAsync(syllabusSchedules);

                // Lưu thay đổi
                int result = await _dbContext.SaveChangesAsync();

                // Commit transaction nếu thành công
                await transaction.CommitAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();


                return false;
            }
        }

        public async Task<bool> UpdateSyllabusScheduleListWithTransactionAsync(List<SyllabusScheduleUpdateItem> items)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in items)
                {
                    var entity = await _dbContext.SyllabusSchedule
                        .FirstOrDefaultAsync(s => s.SyllabusScheduleID == item.SyllabusScheduleID);

                    if (entity == null)
                        continue;

                    entity.Content = item.Content;
                    entity.Resources = item.Resources;
                    entity.LessonTitle = item.LessonTitle;
                    entity.DurationMinutes = item.DurationMinutes;
                    entity.HasTest = item.HasTest;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<SyllabusScheduleCreateLessonDTO>> GetSchedulesBySubjectIdAsync(string subjectId)
        {
            return await _dbContext.SyllabusSchedule
                .Where(s => s.SubjectID == subjectId && s.IsActive)
                .Select(s => new SyllabusScheduleCreateLessonDTO
                {
                    SyllabusScheduleId = s.SyllabusScheduleID,
                    Week = (int)s.Week,
                    DurationMinutes = (int)s.DurationMinutes,
                    HasTest = s.HasTest

                })
                .ToListAsync();
        }
        public async Task<int> GetNumbeOfSyllabusScheduleAsync()
        {
            return await _dbContext.SyllabusSchedule.CountAsync() + 1;
        }
        public async Task<bool> CreateSyllabusesScheduleAsync(SyllabusSchedule syllabusSchedule)
        {
            try
            {
                _dbContext.SyllabusSchedule.Add(syllabusSchedule);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<bool> IsMaxSlotInWeek(string subjectID, int week)
        //{
        //    var count = await _dbContext.SyllabusSchedule
        //        .Where(s => s.subjectID == subjectID && s.Week == week)
        //        .CountAsync();

        //    return count < 8; 
        //}
        public async Task<List<SyllabusSchedule>> GetSyllabusSchedulesBySyllabusIdAsync(string subjectID)
        {
            return await _dbContext.SyllabusSchedule
                .Where(s => s.SubjectID == subjectID)
                .ToListAsync();
        }

        public async Task<bool> SlotAllowToTestAsync(string syllabusScheduleID)
        {
            return await _dbContext.SyllabusSchedule
                     .AnyAsync(x => x.SyllabusScheduleID == syllabusScheduleID
                                 && (x.IsActive == true)
                                 && (x.HasTest == false));
        }



        public async Task<bool> ValidateTestPositionAsync(string subjectID, string syllabusScheduleId, TestCategory testCategory)
        {
            // Lấy tất cả syllabus schedules đang active và có thứ tự
            var syllabusSchedules = await _dbContext.SyllabusSchedule
                         .Where(s => s.SubjectID == subjectID && s.IsActive == true)
                         .OrderBy(s => s.Week)
                         .ThenBy(s => s.SyllabusScheduleID)
                         .Select(s => new
                         {
                             s.SyllabusScheduleID,
                             s.Week
                         })
                         .ToListAsync();

            if (!syllabusSchedules.Any())
                return false;

            // Tìm vị trí của schedule hiện tại
            var currentScheduleIndex = syllabusSchedules.FindIndex(s => s.SyllabusScheduleID == syllabusScheduleId);
            if (currentScheduleIndex == -1)
                return false;

            var isFirstSlot = currentScheduleIndex == 0;

            // Lấy thông tin về các bài kiểm tra hiện có
            var existingTests = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SubjectID == subjectID && t.SyllabusSchedule.IsActive)
                .Select(t => new
                {
                    t.TestCategory,
                    ScheduleId = t.SyllabusSchedule.SyllabusScheduleID
                })
                .ToListAsync();

            var midtermTests = existingTests.Where(t => t.TestCategory == TestCategory.Midterm).ToList();
            var finalTests = existingTests.Where(t => t.TestCategory == TestCategory.Final).ToList();

            switch (testCategory)
            {
                case TestCategory.Midterm:
                    // Midterm không được ở đầu hoặc cuối
                    if (currentScheduleIndex == 0 || currentScheduleIndex == syllabusSchedules.Count - 1)
                        return false;

                    // Midterm không được nằm sau bất kỳ Final nào
                    if (finalTests.Any())
                    {
                        var earliestFinalIndex = finalTests
                            .Select(f => syllabusSchedules.FindIndex(s => s.SyllabusScheduleID == f.ScheduleId))
                            .Min();

                        if (currentScheduleIndex >= earliestFinalIndex)
                            return false;
                    }
                    break;

                case TestCategory.Final:
                    // ❌ Không được ở slot đầu tiên
                    if (isFirstSlot)
                        return false;

                    // ✅ Phải nằm sau tất cả Midterm (nếu có)
                    if (midtermTests.Any())
                    {
                        var latestMidtermIndex = midtermTests
                            .Select(m => syllabusSchedules.FindIndex(s => s.SyllabusScheduleID == m.ScheduleId))
                            .Max();

                        if (currentScheduleIndex <= latestMidtermIndex)
                            return false;
                    }
                    break;

                default:
                    // Các test khác phải nằm trước tất cả bài Final
                    if (finalTests.Any())
                    {
                        var earliestFinalIndex = finalTests
                            .Select(f => syllabusSchedules.FindIndex(s => s.SyllabusScheduleID == f.ScheduleId))
                            .Min();

                        if (currentScheduleIndex >= earliestFinalIndex)
                            return false;
                    }
                    break;
            }

            return true;
        }

        // Method phụ trợ để lấy danh sách tests theo thứ tự (đã sửa đổi từ method gốc)
        public async Task<List<(int? Week, string TestType, TestCategory TestCategory)>> GetActiveTestsOrderedByWeekAsync(string subjectID)
        {
            var tempList = await _dbContext.SyllabusScheduleTests
                 .Where(t => t.SyllabusSchedule.SubjectID == subjectID
                          && t.SyllabusSchedule.IsActive == true)
                 .OrderBy(t => t.SyllabusSchedule.Week)
                 .ThenBy(t => t.SyllabusSchedule.SyllabusScheduleID)
                .Select(t => new
                {
                    Week = (int?)t.SyllabusSchedule.Week,
                    TestType = t.TestType.ToString().ToLower(),
                    TestCategory = t.TestCategory
                })

                 .ToListAsync();

            return tempList.Select(t => (t.Week, t.TestType, t.TestCategory)).ToList();
        }



        public async Task<List<SyllabusSchedule>> GetSyllabusSchedulesByIdsAsync(List<string> ids)
        {
            return await _dbContext.SyllabusSchedule
                        .Where(s => ids.Contains(s.SyllabusScheduleID))
                        .ToListAsync();
        }

        public async Task<SyllabusSchedule> GetSyllabusScheduleByIdAsync(string id)
        {
            return await _dbContext.SyllabusSchedule
                        .FirstOrDefaultAsync(s => s.SyllabusScheduleID == id);
        }

        public async Task UpdateSyllabusScheduleAsync(SyllabusSchedule syllabusSchedule)
        {
            _dbContext.SyllabusSchedule.Update(syllabusSchedule);
        }

        public async Task UpdateSyllabusSchedulesAsync(List<SyllabusSchedule> syllabusSchedules)
        {
            _dbContext.SyllabusSchedule.UpdateRange(syllabusSchedules);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SyllabusSchedule>> GetSchedulesBySubjectAndWeekAsync(string subjectId, int? week)
        {
            var query = _dbContext.SyllabusSchedule
                           .Where(s => s.SubjectID == subjectId);

            if (week.HasValue)
            {
                query = query.Where(s => s.Week == week.Value);
            }

            return await query.OrderBy(s => s.Week).ThenBy(s => s.SyllabusScheduleID).ToListAsync();
        }
    }
}