using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
  
    public class UpdateTestSchedulesToSlotsCommand : IRequest<bool>
    {
        public string SyllabusScheduleID { get; set; }
        public string TestCategory { get; set; }
        public string TestType { get; set; }

        public string syllabusId { get; set; }

        public int SyllabusScheduleTestsId { get; set; }


    }
}
