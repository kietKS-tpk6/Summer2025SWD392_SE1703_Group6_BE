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
            _syllabusScheduleTestService= syllabusScheduleTestService;
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


        public async Task<bool> CreateEmptySyllabusScheduleAyncs(SyllabusScheduleCreateCommand command)
        {
            var numberOfSS = (await _syllabusScheduleRepository.GetNumbeOfSyllabusScheduleAsync());
            var syllabusScheduleList = new List<SyllabusSchedule>();

            // Tổng số SyllabusSchedule cần tạo = SlotInWeek * Week
            int totalSlots = command.SlotInWeek * command.Week;

            // Tạo vòng lặp để tạo ra list SyllabusSchedule rỗng theo tổng số slot
            for (int i = 0; i < totalSlots; i++)
            {
                // Tính toán week hiện tại (bắt đầu từ 1)
                int currentWeek = (i / command.SlotInWeek) + 1;

                // Tạo ID mới cho mỗi SyllabusSchedule
                string newId = "SS" + (numberOfSS + i).ToString("D5");

                var syllabus = new SyllabusSchedule
                {
                    SyllabusScheduleID = newId,
                    SubjectID = command.SubjectID,
                    Week = currentWeek,
                    IsActive = false,
                    // Các thuộc tính khác để null hoặc giá trị mặc định
                    Content = null,
                    Resources = null,
                    LessonTitle = null,
                    DurationMinutes = null,
                    HasTest = false
                };

                syllabusScheduleList.Add(syllabus);
            }

            // Lưu tất cả SyllabusSchedule trong list
            var res = await _syllabusScheduleRepository.CreateMultipleSyllabusesScheduleAsync(syllabusScheduleList);
            return res;
        }
        //public async Task<bool> IsMaxSlotInWeek(string subjectID, int week)
        // {
        //     return await _syllabusScheduleRepository.IsMaxSlotInWeek(subjectID, week);
        //     }


        //kiểm tra slot đó cos được thêm bài kiểm tra hay ko
          public async Task<bool> slotAllowToTestAsync(string syllabusSchedulesID)
        {
          return await  _syllabusScheduleRepository.SlotAllowToTestAsync(syllabusSchedulesID);

        }

       

        public async Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, string testCategory)
        {
            var normalizedTestCategory = _syllabusScheduleTestService.NormalizeTestCategory(testCategory);

            if (normalizedTestCategory == null)
            {
                return false;
            }

            return await _syllabusScheduleRepository.ValidateTestPositionAsync(syllabusId, syllabusScheduleId, normalizedTestCategory.Value);
        }

        public async Task<bool> CheckListSyllabusScheduleAsync(List<SyllabusScheduleUpdateItem> items)
        {
            return await _syllabusScheduleRepository.UpdateSyllabusScheduleListWithTransactionAsync(items);
        }


        //public Task AddTestSchedulesToSlotsAsync(string syllabusId, TestCategory category, TestType testType)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<bool> UpdateSyllabusSchedulesAsync(SyllabusScheduleUpdateCommand command)
        {
            try
            {
                // Lấy danh sách ID từ command
                var ids = command.Items.Select(x => x.SyllabusScheduleID).ToList();

                // Lấy danh sách schedule từ database
                var existingSchedules = await _syllabusScheduleRepository.GetSyllabusSchedulesByIdsAsync(ids);

                // Tạo dictionary để lookup nhanh
                var scheduleDict = existingSchedules.ToDictionary(x => x.SyllabusScheduleID, x => x);

                // Update từng schedule
                foreach (var item in command.Items)
                {
                    if (scheduleDict.TryGetValue(item.SyllabusScheduleID, out var schedule))
                    {
                        UpdateScheduleFromItem(schedule, item);
                    }
                }

                // Cập nhật tất cả vào database
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
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(item.SyllabusScheduleID))
                return false;

            if (string.IsNullOrWhiteSpace(item.LessonTitle))
                return false;

            if (item.DurationMinutes <= 0)
                return false;

            // Có thể thêm các validation khác tùy theo business logic
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
    }
}
