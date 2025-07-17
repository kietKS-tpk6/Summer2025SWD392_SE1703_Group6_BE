using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class SendCertificateToStudentCommand : IRequest<OperationResult<bool>>
    {
        public string StudentID { get; set; } = null!;
        public string ClassID { get; set; } = null!;
    }
}
