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
        private readonly IClassRepository _classRepository;
        private readonly ISubjectService _subjectService;
        public SyllabusScheduleService(
            ISyllabusScheduleRepository syllabusScheduleRepository,
            ISyllabusScheduleTestService syllabusScheduleTestService,
            IAssessmentCriteriaService assessmentCriteriaService,
            HangulLearningSystemDbContext dbContext,
            IClassRepository classRepository,
            ISubjectService subjectService
            )
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
            _syllabusScheduleTestService = syllabusScheduleTestService;
            _assessmentCriteriaService = assessmentCriteriaService;
            _dbContext = dbContext;
            _classRepository = classRepository;
            _subjectService = subjectService;
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
                var result = new List<SyllabusScheduleDTO>();

                foreach (var schedule in schedules)
                {
                    var dto = new SyllabusScheduleDTO
                    {
                        SyllabusScheduleID = schedule.SyllabusScheduleID,
                        Content = schedule.Content,
                        SubjectID = schedule.SubjectID,
                        Resources = schedule.Resources,
                        LessonTitle = schedule.LessonTitle,
                        DurationMinutes = schedule.DurationMinutes.GetValueOrDefault(),
                        HasTest = schedule.HasTest,
                        Week = schedule.Week.GetValueOrDefault(),
                    };

                    // Nếu có test thì lấy thêm test data
                    if (schedule.HasTest)
                    {
                        dto.TestData = await _syllabusScheduleTestService.GetTestDataByScheduleIdAsync(schedule.SyllabusScheduleID);
                    }

                    result.Add(dto);
                }

                return result;
            }
            catch (Exception)
            {
                return new List<SyllabusScheduleDTO>();
            }
        }
        public async Task<OperationResult<List<SyllabusScheduleDTO>>> GetScheduleBySubjectIdAsync(string subjectId)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
                if(subject == null) return OperationResult<List<SyllabusScheduleDTO>>.Fail(OperationMessages.NotFound("môn học"));
                var schedules = await _syllabusScheduleRepository.GetSchedulesBySubjectAndWeekAsync(subjectId, null);
                var result = new List<SyllabusScheduleDTO>();
                foreach (var schedule in schedules)
                {
                    var dto = new SyllabusScheduleDTO
                    {
                        SyllabusScheduleID = schedule.SyllabusScheduleID,
                        Content = schedule.Content,
                        SubjectID = schedule.SubjectID,
                        Resources = schedule.Resources,
                        LessonTitle = schedule.LessonTitle,
                        DurationMinutes = schedule.DurationMinutes.GetValueOrDefault(),
                        HasTest = schedule.HasTest,
                        Week = schedule.Week.GetValueOrDefault(),
                    };


                    result.Add(dto);
                }

                return OperationResult<List<SyllabusScheduleDTO>>.Ok(result,OperationMessages.RetrieveSuccess("lịch học"));
            }
            catch (Exception)
            {
                return OperationResult<List<SyllabusScheduleDTO>>.Fail("Không tìm thấy lịch học.");
            }
        }
        // ========== REFACTORED METHOD - TWO STEP PROCESSING ==========
        public async Task<OperationResult<List<SyllabusScheduleWithSlotDTO>>> CreateEmptySyllabusScheduleAyncs(SyllabusScheduleCreateCommand command)
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

            return OperationResult<List<SyllabusScheduleWithSlotDTO>>.Ok(result, "Xử lý hoàn tất.");
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

        private async Task<OperationResult<List<SyllabusScheduleWithSlotDTO>>> CreateNewSchedulesAsync(SyllabusScheduleCreateCommand command)
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

            return OperationResult<List<SyllabusScheduleWithSlotDTO>>.Ok(result, "Tạo mới hoàn tất.");
        }

        // HELPER METHOD
        private List<SyllabusScheduleWithSlotDTO> MapToSlots(List<SyllabusSchedule> syllabusSchedules)
        {
            return syllabusSchedules
                .OrderBy(s => s.Week)
                .ThenBy(s => s.SyllabusScheduleID)
                .Select((s, idx) => new SyllabusScheduleWithSlotDTO
                {
                    SyllabusScheduleID = s.SyllabusScheduleID,
                    Slot = idx + 1
                })
                .ToList();
        }


        public async Task<OperationResult<bool>> UpdateBulkScheduleWithTestAsync(
         string subjectId,
         List<SyllabusScheduleUpdateItemDTO> scheduleItems)
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
                        var criteriaId = item.ItemsAssessmentCriteria.AssessmentCriteriaID;

                        if (string.IsNullOrWhiteSpace(criteriaId))
                            return OperationResult<bool>.Fail($"AssessmentCriteriaID không được để trống cho slot {item.SyllabusScheduleID}");

                        bool isDuplicate = await _syllabusScheduleTestService
                            .IsDuplicateTestTypeAsync(criteriaId, item.ItemsAssessmentCriteria.TestType);

                        if (isDuplicate)
                        {
                            return OperationResult<bool>.Fail(
                                $"Bài kiểm tra dạng {item.ItemsAssessmentCriteria.TestType} đã tồn tại cho tiêu chí đánh giá {criteriaId}");
                        }

                        var test = new SyllabusScheduleTest
                        {
                            ScheduleTestID = await _syllabusScheduleTestService.GenerateNewScheduleTestIdAsync(),
                            SyllabusScheduleID = schedule.SyllabusScheduleID,
                            TestType = item.ItemsAssessmentCriteria.TestType,
                            IsActive = true,
                            AllowMultipleAttempts = true,
                            AssessmentCriteriaID = criteriaId,
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

        public OperationResult<bool> ValidateTestTypeDuplicatedInInput(IEnumerable<SyllabusScheduleUpdateItemDTO> items)
        {
            var testByCriteria = new Dictionary<(string assessmentCriteriaID, TestType testType), int>();

            foreach (var item in items)
            {
                if (item.HasTest && item.ItemsAssessmentCriteria != null)
                {
                    var criteriaId = item.ItemsAssessmentCriteria.AssessmentCriteriaID;
                    var testType = item.ItemsAssessmentCriteria.TestType;

                    if (string.IsNullOrWhiteSpace(criteriaId))
                    {
                        return OperationResult<bool>.Fail("Thiếu AssessmentCriteriaID trong input.");
                    }

                    var key = (assessmentCriteriaID: criteriaId, testType: testType);

                    if (testByCriteria.ContainsKey(key))
                    {
                        return OperationResult<bool>.Fail(
                            $"TestType '{key.testType}' bị trùng trong tiêu chí đánh giá '{key.assessmentCriteriaID}'.");
                    }

                    testByCriteria[key] = 1;
                }
            }

            return OperationResult<bool>.Ok(true);
        }
        public async Task<OperationResult<List<SyllabusScheduleResourceDTO>>> GetScheduleResourcesByClassIdAsync(string classId)
        {
            var subjectIDResult = await _classRepository.GetSubjectIDByOngoingClassID(classId);

            var schedules = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(subjectIDResult.Data);

            if (schedules == null || !schedules.Any())
                return OperationResult<List<SyllabusScheduleResourceDTO>>.Fail("Không có lịch học nào cho môn học này.");

            var result = schedules.Select(s => new SyllabusScheduleResourceDTO
            {
                SyllabusScheduleID = s.SyllabusScheduleID,
                Resources = s.Resources
            }).ToList();

            return OperationResult<List<SyllabusScheduleResourceDTO>>.Ok(result, OperationMessages.RetrieveSuccess("resources lịch học"));
        }

        public async Task<OperationResult<string?>> GetResourcesByScheduleIDAsync(string syllabusScheduleID)
        {
            var result = await _syllabusScheduleRepository.GetByIdAsync(syllabusScheduleID);
            if (!result.Success || result.Data == null)
                return OperationResult<string?>.Fail($"Không tìm thấy slot với ID {syllabusScheduleID}");

            return OperationResult<string?>.Ok(result.Data.Resources);
        }
    }
}