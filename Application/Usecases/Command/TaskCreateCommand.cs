using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class TaskCreateCommand : IRequest<OperationResult<string?>>
    {
        public string LecturerID { get; set; }
        public TaskType Type { get; set; }
        public string Content { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime Deadline { get; set; }
        public string? Note { get; set; }
        public string? ResourcesURL { get; set; }
    }
}