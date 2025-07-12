using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class GradeWritingAnswerCommand : IRequest<OperationResult<bool>>
    {
        public string WritingAnswerID { get; set; }
        public string StudentTestID { get; set; }
        public decimal WritingScore { get; set; }
        public string Feedback { get; set; }
        public string GraderAccountID { get; set; }
        public string TestSectionID { get; set; } 


    }
}
