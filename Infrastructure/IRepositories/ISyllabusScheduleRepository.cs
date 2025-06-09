using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.IRepositories
{
    public interface ISyllabusScheduleRepository
    {
        Task<List<int>> GetWeeksBySubjectIdAsync(string syllabusId);
        Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId);
      
        Task<bool> CreateSyllabusesScheduleAsync(SyllabusSchedule syllabusSchedule);
        Task<int> GetNumbeOfSyllabusScheduleAsync();

        Task<bool> IsMaxSlotInWeek(string SyllabusID,int week);
        Task<List<SyllabusSchedule>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId);
        Task<bool> SlotAllowToTestAsync( string syllabusId);
        // Task<List<(int Week, string TestType)>> GetActiveTestsOrderedByWeekAsync(string syllabusId);
        Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, TestCategory testCategory);

    }
}
