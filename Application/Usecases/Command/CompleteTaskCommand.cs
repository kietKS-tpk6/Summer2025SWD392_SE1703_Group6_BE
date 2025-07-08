using Application.Common.Constants;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Usecases.Command
{
    public class CompleteTaskCommand : IRequest<OperationResult<string?>>
    {
        public string TaskId { get; set; }

        public string LecturerID { get; set; } 

    }
}