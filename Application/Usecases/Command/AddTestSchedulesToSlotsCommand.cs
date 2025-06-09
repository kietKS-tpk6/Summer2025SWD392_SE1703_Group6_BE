using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.Command
{
    public class AddTestSchedulesToSlotsCommand
    {  
        public string SyllabusScheduleID { get; set; }
        public string TestCategory  { get; set; }
        public string TestType { get; set; }


    }
}
