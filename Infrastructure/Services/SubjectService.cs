using Application.DTOs;
using Application.IRepositories;
using Application.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<List<SubjectDTO>> GetAllSubjectsAsync(bool? isActive = null)
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync(isActive);

            return subjects.Select(s => new SubjectDTO
            {
                SubjectID = s.SubjectID,
                SubjectName = s.SubjectName,
                Description = s.Description,
                IsActive = s.IsActive,
                CreateAt = s.CreateAt,
                MinAverageScoreToPass = s.MinAverageScoreToPass
            }).ToList();
        }

        public async Task<SubjectDTO> GetSubjectByIdAsync(string subjectId)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(subjectId);

            if (subject == null)
                return null;

            return new SubjectDTO
            {
                SubjectID = subject.SubjectID,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                IsActive = subject.IsActive,
                CreateAt = subject.CreateAt,
                MinAverageScoreToPass = subject.MinAverageScoreToPass
            };
        }

        public async Task<bool> SubjectExistsAsync(string subjectId)
        {
            return await _subjectRepository.SubjectExistsAsync(subjectId);
        }

        public async Task<int> GetTotalSubjectsCountAsync()
        {
            return await _subjectRepository.GetTotalSubjectsCountAsync();
        }
    }
}