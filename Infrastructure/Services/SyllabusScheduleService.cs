using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Infrastructure.IRepositories;
namespace Infrastructure.Services
{
    public class SyllabusScheduleService : ISyllabusScheduleService
    {
        private readonly ISyllabusScheduleRepository _syllabusScheduleRepository;

        public SyllabusScheduleService(ISyllabusScheduleRepository syllabusScheduleRepository)
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
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
    }
}
