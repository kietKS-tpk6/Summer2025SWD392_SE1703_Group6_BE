using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Application.Usecases.Command;
using Domain.Enums;

namespace Application.IServices
{
    public interface ISyllabusScheduleService
    {
        public Task<bool> CreateEmptySyllabusScheduleAyncs(SyllabusScheduleCreateCommand Command);

        //Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId);
        Task<int> GetMaxSlotPerWeekAsync(string subjectId);
      //  Task<bool> IsMaxSlotInWeek(string syllabusId,int week);
        Task<List<SyllabusScheduleDTO>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId);
        //Task AddTestSchedulesToSlotsAsync(string syllabusId,TestCategory category,TestType testType);
        Task<bool> slotAllowToTestAsync(string syllabusSchedulesID);
        Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, string testCategory);
        Task<bool> CheckListSyllabusScheduleAsync(List<SyllabusScheduleUpdateItem> items);
        Task<bool> UpdateSyllabusSchedulesAsync(SyllabusScheduleUpdateCommand command);
        Task<List<SyllabusScheduleDTO>> GetScheduleBySubjectAndWeekAsync(string subjectId, int? week);

    }
}
