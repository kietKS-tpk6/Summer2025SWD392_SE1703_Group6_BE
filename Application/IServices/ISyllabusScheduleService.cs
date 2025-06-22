using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Application.Usecases.Command;
using Domain.Enums;
using Application.Common.Constants;

namespace Application.IServices
{
    public interface ISyllabusScheduleService
    {
        Task<OperationResult<List<SyllabusScheduleWithSlotDTO>>> CreateEmptySyllabusScheduleAyncs(SyllabusScheduleCreateCommand command);

        Task<OperationResult<List<SyllabusScheduleCreateLessonDTO>>> GetSchedulesBySubjectIdAsync(string subjectId);

        Task<OperationResult<int>> GetMaxSlotPerWeekAsync(string subjectId);
      //  Task<bool> IsMaxSlotInWeek(string syllabusId,int week);
        //Task AddTestSchedulesToSlotsAsync(string syllabusId,TestCategory category,TestType testType);
        Task<bool> slotAllowToTestAsync(string syllabusSchedulesID);
        //Task<bool> ValidateTestPositionAsync(string syllabusId, string syllabusScheduleId, string testCategory);
        Task<bool> CheckListSyllabusScheduleAsync(List<SyllabusScheduleUpdateItem> items);
        Task<bool> UpdateSyllabusSchedulesAsync(SyllabusScheduleUpdateCommand command);
        Task<List<SyllabusScheduleDTO>> GetScheduleBySubjectAndWeekAsync(string subjectId, int? week);
        Task<OperationResult<bool>> UpdateBulkScheduleWithTestAsync(string subjectId, List<SyllabusScheduleUpdateItemDTO> scheduleItems);
        OperationResult<bool> ValidateTestTypeDuplicatedInInput(IEnumerable<SyllabusScheduleUpdateItemDTO> items);
    }
}
