using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Services
{
    public class SyllabusScheduleService : ISyllabusScheduleService
    {
        private readonly ISyllabusScheduleRepository _syllabusScheduleRepository;
        private readonly ISyllabusesRepository _syllabusesRepository;

        public SyllabusScheduleService(
            ISyllabusScheduleRepository syllabusScheduleRepository,
            ISyllabusesRepository syllabusesRepository)
        {
            _syllabusScheduleRepository = syllabusScheduleRepository;
            _syllabusesRepository = syllabusesRepository;
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
            var existsSyllabus = await _syllabusesRepository.ExistsSyllabusAsync(command.SyllabusID);
            if (!existsSyllabus) throw new ArgumentException("SyllabusID không tồn tại.");

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
            var res = await _syllabusScheduleRepository.CreateSyllabusesScheduleAsync(syllabus);
            return res;

        }
    }
}
