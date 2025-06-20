using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Constants;
using Domain.Enums;
using Domain.Entities;
namespace Application.Usecases.Command
{
    public class CreateQuestionsCommand : IRequest<OperationResult<List<Question>>>
    {
        public string TestSectionID { get; set; }
        public TestFormatType FormatType { get; set; }
        public int NumberOfQuestions { get; set; }
        public float Score { get; set; }

    }
}
