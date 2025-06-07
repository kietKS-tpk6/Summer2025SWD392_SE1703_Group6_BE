using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.IServices
{
    public interface ISyllabusScheduleService
    {
        Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId);
        Task<int> GetMaxSlotPerWeekAsync(string syllabusId);
    }
}
