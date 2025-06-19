using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Services
{
    public class SyllabusScheduleService : ISyllabusScheduleService
    {
        private readonly HangulLearningSystemDbContext _dbContext;
        private readonly ISyllabusScheduleRepository _syllabusScheduleRepository;
        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;

        public SyllabusScheduleService(
            ISyllabusScheduleRepository syllabusScheduleRepository,
            ISyllabusScheduleTestService syllabusScheduleTestService,
            IAssessmentCriteriaService assessmentCriteriaService,
            HangulLearningSystemDbContext dbContext)
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
            _syllabusScheduleTestService = syllabusScheduleTestService;
            _assessmentCriteriaService = assessmentCriteriaService;
            _dbContext = dbContext;
        }

        public async Task<OperationResult<int>> GetMaxSlotPerWeekAsync(string subjectId)
        {
            var result = await _syllabusScheduleRepository.GetWeeksBySubjectIdAsync(subjectId);

            if (!result.Success || result.Data == null)
                return OperationResult<int>.Fail(result.Message ?? "Không thể lấy danh sách tuần học");

            var weeks = result.Data;

            var grouped = weeks.GroupBy(w => w)
                               .Select(g => g.Count());

            int max = grouped.Any() ? grouped.Max() : 0;

            return OperationResult<int>.Ok(max, OperationMessages.RetrieveSuccess("số tiết tối đa mỗi tuần"));
        }

        public async Task<OperationResult<List<SyllabusScheduleCreateLessonDTO>>> GetSchedulesBySubjectIdAsync(string subjectId)
        {
            var schedules = await _syllabusScheduleRepository.GetSchedulesBySubjectIdAsync(subjectId);

            if (schedules == null || !schedules.Any())
            {
                return OperationResult<List<SyllabusScheduleCreateLessonDTO>>
                    .Fail(OperationMessages.NotFound("lịch học"));
            }

            return OperationResult<List<SyllabusScheduleCreateLessonDTO>>
                .Ok(schedules, OperationMessages.RetrieveSuccess("lịch học"));
        }

        public async Task<bool> slotAllowToTestAsync(string syllabusSchedulesID)
        {
            return await _syllabusScheduleRepository.SlotAllowToTestAsync(syllabusSchedulesID);
        }

        //public async Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, string testCategory)
        //{
        //    var normalizedTestCategory = _syllabusScheduleTestService.NormalizeTestCategory(testCategory);

        //    if (normalizedTestCategory == null)
        //    {
        //        return false;
        //    }

        //    return await _syllabusScheduleRepository.ValidateTestPositionAsync(syllabusId, syllabusScheduleId, normalizedTestCategory.Value);
        //}

        public async Task<bool> CheckListSyllabusScheduleAsync(List<SyllabusScheduleUpdateItem> items)
        {
            return await _syllabusScheduleRepository.UpdateSyllabusScheduleListWithTransactionAsync(items);
        }

        public async Task<bool> UpdateSyllabusSchedulesAsync(SyllabusScheduleUpdateCommand command)
        {
            try
            {
                var ids = command.Items.Select(x => x.SyllabusScheduleID).ToList();
                var existingSchedules = await _syllabusScheduleRepository.GetSyllabusSchedulesByIdsAsync(ids);
                var scheduleDict = existingSchedules.ToDictionary(x => x.SyllabusScheduleID, x => x);

                foreach (var item in command.Items)
                {
                    if (scheduleDict.TryGetValue(item.SyllabusScheduleID, out var schedule))
                    {
                        UpdateScheduleFromItem(schedule, item);
                    }
                }

                await _syllabusScheduleRepository.UpdateSyllabusSchedulesAsync(existingSchedules);
                await _syllabusScheduleRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidSyllabusScheduleItem(SyllabusScheduleUpdateItem item)
        {
            if (string.IsNullOrWhiteSpace(item.SyllabusScheduleID))
                return false;

            if (string.IsNullOrWhiteSpace(item.LessonTitle))
                return false;

            if (item.DurationMinutes <= 0)
                return false;

            return true;
        }

        private void UpdateScheduleFromItem(SyllabusSchedule schedule, SyllabusScheduleUpdateItem item)
        {
            schedule.Content = item.Content;
            schedule.Resources = item.Resources;
            schedule.LessonTitle = item.LessonTitle;
            schedule.DurationMinutes = item.DurationMinutes;
            schedule.HasTest = item.HasTest;
        }

        public async Task<List<SyllabusScheduleDTO>> GetScheduleBySubjectAndWeekAsync(string subjectId, int? week)
        {
            try
            {
                var schedules = await _syllabusScheduleRepository.GetSchedulesBySubjectAndWeekAsync(subjectId, week);

                return schedules.Select(s => new SyllabusScheduleDTO
                {
                    SyllabusScheduleID = s.SyllabusScheduleID,
                    Content = s.Content,
                    SubjectID = s.SubjectID,
                    Resources = s.Resources,
                    LessonTitle = s.LessonTitle,
                    DurationMinutes = s.DurationMinutes.GetValueOrDefault(),
                    HasTest = s.HasTest,
                    Week = s.Week.GetValueOrDefault(),
                }).ToList();
            }
            catch (Exception)
            {
                return new List<SyllabusScheduleDTO>();
            }
        }

        // ========== REFACTORED METHOD - TWO STEP PROCESSING ==========
        public async Task<OperationResult<List<SyllabusScheduleWithSlotDto>>> CreateEmptySyllabusScheduleAyncs(SyllabusScheduleCreateCommand command)
        {
            var existing = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);

            if (existing == null || !existing.Any())
            {
                return await CreateNewSchedulesAsync(command);
            }

            await ProcessWeeksAsync(existing, command);

            var afterWeekProcessing = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);

            await ProcessSlotsAsync(afterWeekProcessing, command);

            var finalResult = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);
            var result = MapToSlots(finalResult);

            return OperationResult<List<SyllabusScheduleWithSlotDto>>.Ok(result, "Xử lý hoàn tất.");
        }

        private async Task ProcessWeeksAsync(List<SyllabusSchedule> existing, SyllabusScheduleCreateCommand command)
        {
            int existingWeeks = existing.Select(s => s.Week).Distinct().Count();
            int targetWeeks = command.week;

            if (existingWeeks == targetWeeks)
            {
                return;
            }
            else if (existingWeeks < targetWeeks)
            {
                await AddWeeksAsync(existing, command, existingWeeks, targetWeeks);
            }
            else // existingWeeks > targetWeeks
            {
                await RemoveWeeksAsync(existing, targetWeeks);
            }
        }

        private async Task AddWeeksAsync(List<SyllabusSchedule> existing, SyllabusScheduleCreateCommand command, int existingWeeks, int targetWeeks)
        {
            int numberOfSS = await _syllabusScheduleRepository.GetNumbeOfSyllabusScheduleAsync();
            var newSchedules = new List<SyllabusSchedule>();

            // Tính số slot hiện tại của tuần đầu tiên để biết mẫu
            int slotsPerWeek = existing.Where(s => s.Week == 1).Count();

            // Thêm các tuần mới
            for (int week = existingWeeks + 1; week <= targetWeeks; week++)
            {
                for (int slot = 0; slot < slotsPerWeek; slot++)
                {
                    string newId = "SS" + (++numberOfSS).ToString("D5");

                    newSchedules.Add(new SyllabusSchedule
                    {
                        SyllabusScheduleID = newId,
                        SubjectID = command.subjectID,
                        Week = week,
                        IsActive = true,
                        Content = null,
                        Resources = null,
                        LessonTitle = null,
                        DurationMinutes = null,
                        HasTest = false
                    });
                }
            }

            await _syllabusScheduleRepository.CreateMultipleSyllabusesScheduleAsync(newSchedules);
        }

        private async Task RemoveWeeksAsync(List<SyllabusSchedule> existing, int targetWeeks)
        {
            // Lấy tất cả schedule thuộc các tuần bị xóa
            var schedulesToRemove = existing
                .Where(s => s.Week > targetWeeks)
                .Select(s => s.SyllabusScheduleID)
                .ToList();

            if (schedulesToRemove.Any())
            {
                await _syllabusScheduleRepository.RemoveSyllabusesScheduleAsync(schedulesToRemove);
            }
        }

        private async Task ProcessSlotsAsync(List<SyllabusSchedule> existing, SyllabusScheduleCreateCommand command)
        {
            int existingSlotsPerWeek = existing.Where(s => s.Week == 1).Count();
            int targetSlotsPerWeek = command.slotInWeek;

            if (existingSlotsPerWeek == targetSlotsPerWeek)
            {
                // Không cần làm gì với slot
                return;
            }
            else if (existingSlotsPerWeek < targetSlotsPerWeek)
            {
                // THÊM SLOT
                await AddSlotsAsync(existing, command, existingSlotsPerWeek, targetSlotsPerWeek);
            }
            else // existingSlotsPerWeek > targetSlotsPerWeek
            {
                // XÓA SLOT
                await RemoveSlotsAsync(existing, targetSlotsPerWeek);
            }
        }

        private async Task AddSlotsAsync(List<SyllabusSchedule> existing, SyllabusScheduleCreateCommand command, int existingSlotsPerWeek, int targetSlotsPerWeek)
        {
            int numberOfSS = await _syllabusScheduleRepository.GetNumbeOfSyllabusScheduleAsync();
            var newSchedules = new List<SyllabusSchedule>();

            int slotsToAdd = targetSlotsPerWeek - existingSlotsPerWeek;
            var weeks = existing.Select(s => s.Week).Distinct().OrderBy(w => w).ToList();

            // Thêm slot cho từng tuần
            foreach (var week in weeks)
            {
                for (int i = 0; i < slotsToAdd; i++)
                {
                    string newId = "SS" + (++numberOfSS).ToString("D5");

                    newSchedules.Add(new SyllabusSchedule
                    {
                        SyllabusScheduleID = newId,
                        SubjectID = command.subjectID,
                        Week = week,
                        IsActive = true,
                        Content = null,
                        Resources = null,
                        LessonTitle = null,
                        DurationMinutes = null,
                        HasTest = false
                    });
                }
            }

            await _syllabusScheduleRepository.CreateMultipleSyllabusesScheduleAsync(newSchedules);
        }

        private async Task RemoveSlotsAsync(List<SyllabusSchedule> existing, int targetSlotsPerWeek)
        {
            var schedulesToRemove = new List<string>();
            var weeks = existing.Select(s => s.Week).Distinct().OrderBy(w => w).ToList();

            foreach (var week in weeks)
            {
                var weekSchedules = existing
                    .Where(s => s.Week == week)
                    .OrderBy(s => s.SyllabusScheduleID)
                    .ToList();

                int currentSlots = weekSchedules.Count;
                int slotsToRemove = currentSlots - targetSlotsPerWeek;

                if (slotsToRemove > 0)
                {
                    // Xóa các slot cuối cùng của tuần
                    var idsToRemove = weekSchedules
                        .TakeLast(slotsToRemove)
                        .Select(s => s.SyllabusScheduleID)
                        .ToList();

                    schedulesToRemove.AddRange(idsToRemove);
                }
            }

            if (schedulesToRemove.Any())
            {
                await _syllabusScheduleRepository.RemoveSyllabusesScheduleAsync(schedulesToRemove);
            }
        }

        // TẠO MỚI HOÀN TOÀN
        private async Task<OperationResult<List<SyllabusScheduleWithSlotDto>>> CreateNewSchedulesAsync(SyllabusScheduleCreateCommand command)
        {
            int total = command.slotInWeek * command.week;
            int numberOfSS = await _syllabusScheduleRepository.GetNumbeOfSyllabusScheduleAsync();

            var newSchedules = new List<SyllabusSchedule>();

            for (int i = 0; i < total; i++)
            {
                int currentWeek = (i / command.slotInWeek) + 1;
                string newId = "SS" + (numberOfSS + i).ToString("D5");

                newSchedules.Add(new SyllabusSchedule
                {
                    SyllabusScheduleID = newId,
                    SubjectID = command.subjectID,
                    Week = currentWeek,
                    IsActive = true,
                    Content = null,
                    Resources = null,
                    LessonTitle = null,
                    DurationMinutes = null,
                    HasTest = false
                });
            }

            await _syllabusScheduleRepository.CreateMultipleSyllabusesScheduleAsync(newSchedules);
            var updated = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);
            var result = MapToSlots(updated);

            return OperationResult<List<SyllabusScheduleWithSlotDto>>.Ok(result, "Tạo mới hoàn tất.");
        }

        // HELPER METHOD
        private List<SyllabusScheduleWithSlotDto> MapToSlots(List<SyllabusSchedule> syllabusSchedules)
        {
            return syllabusSchedules
                .OrderBy(s => s.Week)
                .ThenBy(s => s.SyllabusScheduleID)
                .Select((s, idx) => new SyllabusScheduleWithSlotDto
                {
                    SyllabusScheduleID = s.SyllabusScheduleID,
                    Slot = idx + 1
                })
                .ToList();
        }
      

        public async Task<OperationResult<bool>> UpdateBulkScheduleWithTestAsync(
    string subjectId,
    List<SyllabusScheduleUpdateItemDto> scheduleItems)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in scheduleItems)
                {
                    // 1. Lấy ra schedule hiện có
                    var getScheduleResult = await _syllabusScheduleRepository.GetByIdAsync(item.SyllabusScheduleID);
                    if (!getScheduleResult.Success || getScheduleResult.Data == null)
                        return OperationResult<bool>.Fail($"Không tìm thấy slot với ID {item.SyllabusScheduleID}");

                    var schedule = getScheduleResult.Data;

                    // 2. Cập nhật thông tin cơ bản
                    schedule.Content = item.Content;
                    schedule.Resources = item.Resources;
                    schedule.LessonTitle = item.LessonTitle;
                    schedule.DurationMinutes = item.DurationMinutes;
                    schedule.HasTest = item.HasTest;
                    schedule.IsActive = true;

                    var updateResult = await _syllabusScheduleRepository.UpdateAsync(schedule);
                    if (!updateResult.Success)
                        return OperationResult<bool>.Fail($"Lỗi khi cập nhật slot: {updateResult.Message}");

                    // 3. Nếu có bài kiểm tra
                    if (item.HasTest)
                    {
                        var criteriaResult = await _assessmentCriteriaService
                            .GetAssessmentCriteriaIdBySubjectAndOrderAsync(subjectId, item.ItemsAssessmentCriteria.Order);

                        if (!criteriaResult.Success || criteriaResult.Data == null)
                            return OperationResult<bool>.Fail($"Không tìm thấy tiêu chí đánh giá cho order {item.ItemsAssessmentCriteria.Order}");
                        
                        bool isDuplicate = await _syllabusScheduleTestService
                                  .IsDuplicateTestTypeAsync(criteriaResult.Data, item.ItemsAssessmentCriteria.TestType);

                        if (isDuplicate)
                        {
                            return OperationResult<bool>.Fail(
                                $"Bài kiểm tra dạng {item.ItemsAssessmentCriteria.TestType} đã tồn tại cho trong các slot đã thêm ");
                        }
                        var test = new SyllabusScheduleTest
                        {
                            ScheduleTestID = await _syllabusScheduleTestService.GenerateNewScheduleTestIdAsync(),
                            SyllabusScheduleID = schedule.SyllabusScheduleID,  // Giữ lại dòng này
                            TestType = item.ItemsAssessmentCriteria.TestType,
                            IsActive = true,
                            AllowMultipleAttempts = true,
                            AssessmentCriteriaID = criteriaResult.Data,
                            DurationMinutes = item.DurationMinutes
                        };

                        var createTestResult = await _syllabusScheduleTestService.CreateAsync(test);
                        if (!createTestResult.Success)
                            return OperationResult<bool>.Fail($"Lỗi khi tạo kiểm tra cho slot {schedule.SyllabusScheduleID}");
                    }
                }

                await transaction.CommitAsync();
                return OperationResult<bool>.Ok(true, "Cập nhật toàn bộ slot thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult<bool>.Fail("Lỗi hệ thống: " + ex.Message);
            }
        }

        public OperationResult<bool> ValidateTestTypeDuplicatedInInput(IEnumerable<SyllabusScheduleUpdateItemDto> items)
        {
            var testByCriteria = new Dictionary<(int order, TestType testType), int>();

            foreach (var item in items)
            {
                if (item.HasTest && item.ItemsAssessmentCriteria != null)
                {
                    var key = (order: item.ItemsAssessmentCriteria.Order, testType: (TestType)item.ItemsAssessmentCriteria.TestType);

                    if (testByCriteria.ContainsKey(key))
                    {
                        return OperationResult<bool>.Fail(
                            $"TestType '{key.testType}' bị trùng trong tiêu chí đánh giá có order {key.order}");
                    }

                    testByCriteria[key] = 1;
                }
            }

            return OperationResult<bool>.Ok(true);
        }

    }
}