using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private readonly ISyllabusesRepository _syllabusesRepository;
        

        public SyllabusScheduleService(
            ISyllabusScheduleRepository syllabusScheduleRepository,
            ISyllabusesRepository syllabusesRepository,
            ISyllabusScheduleTestService syllabusScheduleTestService)
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
            _syllabusesRepository = syllabusesRepository;
            _syllabusScheduleTestService= syllabusScheduleTestService;
        }

        public async Task<int> GetMaxSlotPerWeekAsync(string syllabusId)
        {
            var weeks = await _syllabusScheduleRepository.GetWeeksBySubjectIdAsync(syllabusId);
            var grouped = weeks.GroupBy(w => w)
                               .Select(g => g.Count());

            return grouped.Any() ? grouped.Max() : 0;
        }
        public async Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId)
        {
            return await _syllabusScheduleRepository.GetPublishedSchedulesBySyllabusIdAsync(syllabusId);
        }
        

        public async Task<bool> CreateSyllabusScheduleAyncs(SyllabusScheduleCreateCommand command)
        {
            

            var numberOfSS = (await _syllabusScheduleRepository.GetNumbeOfSyllabusScheduleAsync());
            string newId = "SS" + numberOfSS.ToString("D5");

            var syllabus = new SyllabusSchedule();
            syllabus.SyllabusID = command.SyllabusID;
            syllabus.Week = command.Week;
            syllabus.Resources = command.Resources;
            syllabus.LessonTitle = command.LessonTitle;
            syllabus.DurationMinutes = command.DurationMinutes;
            syllabus.Content = command.Content;
            syllabus.SyllabusScheduleID = newId;
            syllabus.IsActive= false;
            syllabus.HasTest = command.HasTest;
            var res = await _syllabusScheduleRepository.CreateSyllabusesScheduleAsync(syllabus);
            return res;

        }
       public async Task<bool> IsMaxSlotInWeek(string syllabusId, int week)
        {
            return await _syllabusScheduleRepository.IsMaxSlotInWeek(syllabusId, week);


            }
        public async Task<List<SyllabusScheduleDTO>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId)
        {
            var schedules = await _syllabusScheduleRepository.GetSyllabusSchedulesBySyllabusIdAsync(syllabusId);
            var res = new List<SyllabusScheduleDTO>();

            int slotNumber = 1;

            foreach (var s in schedules)
            {
                var dto = new SyllabusScheduleDTO
                {
                    SyllabusScheduleID = s.SyllabusScheduleID,
                    SyllabusID = s.SyllabusID,
                    Content = s.Content,
                    Week = s.Week,
                    Resources = s.Resources,
                    LessonTitle = s.LessonTitle,
                    DurationMinutes = s.DurationMinutes,
                    IsActive = s.IsActive,
                    HasTest = s.HasTest,
                    Slot = $"Slot {slotNumber++}"
                };

                res.Add(dto);
            }

            return res;
        }

        //kiểm tra slot đó cos được thêm bài kiểm tra hay ko
          public async Task<bool> slotAllowToTestAsync(string syllabusSchedulesID)
        {
          return await  _syllabusScheduleRepository.SlotAllowToTestAsync(syllabusSchedulesID);

        }

        public async Task<bool> ValidateTestOrderAsync(string syllabusId)
        {
            //var tests = await _syllabusScheduleRepository.GetActiveTestsOrderedByWeekAsync(syllabusId);

            //var testTypes = tests.Select(t => t.TestType).ToList();

            //int midtermIndex = testTypes.IndexOf("Midterm");
            //int finalIndex = testTypes.IndexOf("Final");

            //if (finalIndex == -1) return true; // Không có final thì không ràng buộc
            //if (finalIndex != testTypes.Count - 1) return false; // final phải là test cuối cùng
            //if (midtermIndex != -1 && midtermIndex > finalIndex) return false; // midterm phải trước final nếu có

            return true;
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
       


        //public Task AddTestSchedulesToSlotsAsync(string syllabusId, TestCategory category, TestType testType)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
