using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.Command
{
    public class UpdateSyllabusesCommand
    {
        public string SyllabusID { get; set; }
        public string SubjectID { get; set; }

        public string AccountID { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public string Status { get; set; }
    }
}
