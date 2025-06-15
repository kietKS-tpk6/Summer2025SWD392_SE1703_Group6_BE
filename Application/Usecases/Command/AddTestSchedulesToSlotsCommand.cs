using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class AddTestSchedulesToSlotsCommand : IRequest<bool>
    {  
        public string SyllabusScheduleID { get; set; }
        public string TestCategory  { get; set; }
        public string TestType { get; set; }

        public string subjectID { get; set; }


    }
}
