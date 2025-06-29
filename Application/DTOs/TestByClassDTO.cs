using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TestByClassDTO
    {
        public string TestEventID { get; set; }

        public string? TestID { get; set; }

        //kit {Lấy từ bảng Tests thông qua TestID}
        public string? TestCategory { get; set; }

        //kit {Lấy từ bảng Tests thông qua TestID}
        public string? TestName { get; set; }

        public string? Description { get; set; }

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public string TestType { get; set; }

        public string Status { get; set; }
    }

}
