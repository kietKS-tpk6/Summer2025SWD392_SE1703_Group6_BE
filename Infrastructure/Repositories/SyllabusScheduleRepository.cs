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
namespace Infrastructure.Repositories
{
    public class SyllabusScheduleRepository : ISyllabusScheduleRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusScheduleRepository(HangulLearningSystemDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<int>> GetWeeksBySubjectIdAsync(string syllabusId)
        {
            var weeks = await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusId)
                .Select(s => s.Week)
                .ToListAsync();

            return weeks;
        }

        public async Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId)
            {
            var result = await _dbContext.SyllabusSchedule
                .Where(ss => ss.SyllabusID == syllabusId)
                .Select(ss => new SyllabusScheduleCreateLessonDTO
            {
                    SyllabusScheduleId = ss.SyllabusScheduleID,
                    Week = ss.Week,
                    DurationMinutes = ss.DurationMinutes
                })
                .ToListAsync();

            return result;
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

        public async Task<bool> IsMaxSlotInWeek(string syllabusID, int week)
        {
            var count = await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusID && s.Week == week)
                .CountAsync();

            return count < 8; 
        }
        public async Task<List<SyllabusSchedule>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId)
        {
            return await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusId)
                .ToListAsync();
        }

        public async Task<bool> SlotAllowToTestAsync(string syllabusScheduleID)
        {
            return await _dbContext.SyllabusSchedule
                .AnyAsync(x => x.SyllabusScheduleID == syllabusScheduleID
                            && x.IsActive
                            && x.HasTest == true);
        }



        public async Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, TestCategory testCategory)
        {
            // Lấy tất cả syllabus schedules đang active và có thứ tự
            var syllabusSchedules = await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusId && s.IsActive)
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
                .Where(t => t.SyllabusSchedule.SyllabusID == syllabusId && t.SyllabusSchedule.IsActive)
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
        public async Task<List<(int Week, string TestType, TestCategory TestCategory)>> GetActiveTestsOrderedByWeekAsync(string syllabusId)
        {
            var tempList = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SyllabusID == syllabusId && t.SyllabusSchedule.IsActive)
                .OrderBy(t => t.SyllabusSchedule.Week)
                .ThenBy(t => t.SyllabusSchedule.SyllabusScheduleID)
                .Select(t => new
                {
                    Week = t.SyllabusSchedule.Week,
                    TestType = t.TestType.ToString().ToLower(),
                    TestCategory = t.TestCategory
                })
                .ToListAsync();

            return tempList.Select(t => (t.Week, t.TestType, t.TestCategory)).ToList();
        }

    }
}
