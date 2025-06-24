using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class SubmitStudentTestCommand : IRequest<OperationResult<bool>>
    {
        public string StudentTestID { get; set; }
        public string TestID { get; set; }
        public List<SectionAnswerDTO> SectionAnswers { get; set; }
    }
}
