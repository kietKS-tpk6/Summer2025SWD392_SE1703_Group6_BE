using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.IRepositories
{
    public interface ISyllabusScheduleRepository
    {
        Task<OperationResult<List<int>>> GetWeeksBySubjectIdAsync(string subjectId);
        Task<List<SyllabusScheduleCreateLessonDTO>> GetSchedulesBySubjectIdAsync(string subjectId);


        Task<bool> CreateSyllabusesScheduleAsync(SyllabusSchedule syllabusSchedule);
        Task<int> GetNumbeOfSyllabusScheduleAsync();

       // Task<bool> IsMaxSlotInWeek(string subjectID, int week);
        Task<List<SyllabusSchedule>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId);
        Task<bool> SlotAllowToTestAsync( string syllabusId);
        // Task<List<(int Week, string TestType)>> GetActiveTestsOrderedByWeekAsync(string syllabusId);
        Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, TestCategory testCategory);
        Task<bool> CreateMultipleSyllabusesScheduleAsync(List<SyllabusSchedule> syllabusSchedules);
        Task<bool> UpdateSyllabusScheduleListWithTransactionAsync(List<SyllabusScheduleUpdateItem> items);

        Task<List<SyllabusSchedule>> GetSyllabusSchedulesByIdsAsync(List<string> ids);
        Task<SyllabusSchedule> GetSyllabusScheduleByIdAsync(string id);
        Task UpdateSyllabusScheduleAsync(SyllabusSchedule syllabusSchedule);
        Task UpdateSyllabusSchedulesAsync(List<SyllabusSchedule> syllabusSchedules);
        Task SaveChangesAsync();
        Task<List<SyllabusSchedule>> GetSchedulesBySubjectAndWeekAsync(string subjectId, int? week);

    }
}
