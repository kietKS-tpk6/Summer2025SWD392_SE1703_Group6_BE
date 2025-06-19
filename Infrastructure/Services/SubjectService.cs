
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }
       

        public async Task<bool> SubjectExistsAsync(string id)
        {
            return await _subjectRepository.SubjectExistsAsync(id);

        }

        public async Task<string> GenerateNextSubjectIdAsync()
        {
            var allSubjects = await _subjectRepository.GetAllSubjectsAsync();

            if (!allSubjects.Any())
            {
                return "SJ0001";
            }

            var maxId = allSubjects
                .Select(s => s.SubjectID)
                .Where(id => id.StartsWith("SJ") && id.Length == 6)
                .Select(id => int.TryParse(id.Substring(2), out int num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"SJ{(maxId + 1):D4}"; 
        }
        public DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
        public async Task<OperationResult<string>> CreateSubjectAsync(Subject subject)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject.SubjectID))
                {
                    subject.SubjectID = await GenerateNextSubjectIdAsync();
                }

                subject.CreateAt = GetVietnamTime();

                await _subjectRepository.CreateSubjectAsync(subject);
                return OperationResult<string>.Ok(subject.SubjectID,OperationMessages.CreateSuccess("môn học"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Không thể thêm môn học. Lỗi: {ex.Message}");
            }
        }


        public async Task<bool> SubjectNameExistsAsync(string subjectName)
        {
            return await _subjectRepository.ExistsByIdAsync(subjectName);
        }

        public async Task<bool> DescriptionExistsAsync(string description)
        {
            return await _subjectRepository.ExistsByDescriptionAsync(description);
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


        public async Task<int> GetTotalSubjectsCountAsync()
        {
            return await _subjectRepository.GetTotalSubjectsCountAsync();
        }

        public async Task<string> UpdateSubjectAsync(UpdateSubjectCommand command)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(command.SubjectID);
            if (existingSubject == null)
            {
                return $"Subject with ID {command.SubjectID} not found";
            }

            existingSubject.SubjectName = command.SubjectName;
            existingSubject.Description = command.Description;
            existingSubject.IsActive = command.IsActive;
            existingSubject.MinAverageScoreToPass = command.MinAverageScoreToPass;

            return await _subjectRepository.UpdateSubjectAsync(existingSubject);
        }

        public async Task<string> DeleteSubjectAsync(string subjectId)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(subjectId);
            if (existingSubject == null)
            {
                return $"Subject with ID {subjectId} not found";
            }

            return await _subjectRepository.DeleteSubjectAsync(subjectId);
        }
        public async Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus)
        {
            return await _subjectRepository.GetSubjectByStatusAsync(subjectStatus);
        }
    }
}