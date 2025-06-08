using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface ISyllabusesService
    {
        public Task<bool> createSyllabuses(CreateSyllabusesCommand createSyllabusesCommand);
        public Task<string> UpdateSyllabusesAsync(UpdateSyllabusesCommand updateSyllabusesCommand);
        public Task<bool> IsValidSyllabusStatusForSubjectAsync(string subjectID)

    }
}
