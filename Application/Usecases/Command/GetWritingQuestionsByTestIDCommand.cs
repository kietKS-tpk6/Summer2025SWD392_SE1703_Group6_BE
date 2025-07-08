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
    public class GetWritingQuestionsByTestIDCommand : IRequest<OperationResult<List<WritingQuestionWithBaremsDTO>>>
    {
        public string TestID { get; set; }
    }
}
