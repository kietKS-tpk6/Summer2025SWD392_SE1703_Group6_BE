using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface ISyllabusScheduleService
    {
        public Task<bool> CreateSyllabusScheduleAyncs(SyllabusScheduleCreateCommand Command);

    }
}
