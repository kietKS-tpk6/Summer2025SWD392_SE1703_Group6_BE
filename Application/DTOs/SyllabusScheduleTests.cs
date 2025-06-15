using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class SyllabusScheduleTests
    {
        public string SyllabusSchedulesID { get; set; }

        public TestCategory TestCategory { get; set; }

        public TestType TestType { get; set; }

        public bool IsActive { get; set; }
    }
}
