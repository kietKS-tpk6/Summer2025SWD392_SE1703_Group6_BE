using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface ISyllabusesRepository
    {
        Task<int> GetNumbeOfSyllabusAsync();
        Task<bool> CreateSyllabusesAsync(Syllabus syllabus);

        Task<bool> UpdateSyllabusesAsync(Syllabus syllabus);
        Task<bool> ExistsSyllabusAsync(string  syllabusID);
        Task<bool> IsValidSyllabusStatusForSubjectAsync(string SubjectId);
    }
}
