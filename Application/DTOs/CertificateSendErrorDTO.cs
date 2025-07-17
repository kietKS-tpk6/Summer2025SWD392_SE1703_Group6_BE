using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CertificateSendErrorDTO
    {
        public string StudentID { get; set; } = null!;
        public string? FullName { get; set; }
        public string Reason { get; set; } = null!;
    }
}
