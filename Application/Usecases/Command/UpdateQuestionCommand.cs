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
    public class UpdateQuestionCommand : IRequest<OperationResult<bool>>
    {
        public string QuestionID { get; set; }
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public List<MCQOptionUpdateQuestionDTO>? Options { get; set; }
    }
    public class MCQOptionUpdateQuestionDTO
    {
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public bool IsCorrect { get; set; }
    }
}
