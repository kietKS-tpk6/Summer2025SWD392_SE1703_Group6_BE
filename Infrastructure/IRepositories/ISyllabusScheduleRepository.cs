using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

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

    }
}
