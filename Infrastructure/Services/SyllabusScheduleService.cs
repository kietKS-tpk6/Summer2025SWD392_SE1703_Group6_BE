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
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Services
{
    public class SyllabusScheduleService : ISyllabusScheduleService
    {
        private readonly ISyllabusScheduleRepository _syllabusScheduleRepository;
        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;

        public SyllabusScheduleService(
            ISyllabusScheduleRepository syllabusScheduleRepository,
            ISyllabusScheduleTestService syllabusScheduleTestService)
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
            _syllabusScheduleTestService = syllabusScheduleTestService;
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

            // BƯỚC 1: XỬ LÝ TUẦN
            await ProcessWeeksAsync(existing, command);

            // Lấy lại data sau khi xử lý tuần
            var afterWeekProcessing = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);

            // BƯỚC 2: XỬ LÝ SLOT
            await ProcessSlotsAsync(afterWeekProcessing, command);

            // Lấy kết quả cuối cùng
            var finalResult = await _syllabusScheduleRepository.GetSyllabusSchedulesBySubjectIdAsync(command.subjectID);
            var result = MapToSlots(finalResult);

            return OperationResult<List<SyllabusScheduleWithSlotDto>>.Ok(result, "Xử lý hoàn tất.");
        }

        // BƯỚC 1: XỬ LÝ TUẦN
        private async Task ProcessWeeksAsync(List<SyllabusSchedule> existing, SyllabusScheduleCreateCommand command)
        {
            int existingWeeks = existing.Select(s => s.Week).Distinct().Count();
            int targetWeeks = command.week;

            if (existingWeeks == targetWeeks)
            {
                // Không cần làm gì với tuần
                return;
            }
            else if (existingWeeks < targetWeeks)
            {
                // THÊM TUẦN
                await AddWeeksAsync(existing, command, existingWeeks, targetWeeks);
            }
            else // existingWeeks > targetWeeks
            {
                // XÓA TUẦN
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

        // BƯỚC 2: XỬ LÝ SLOT
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
    }
}