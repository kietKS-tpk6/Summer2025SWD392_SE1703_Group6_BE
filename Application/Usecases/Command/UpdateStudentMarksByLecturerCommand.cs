using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Commands
{
    public class UpdateStudentMarksByLecturerCommand : IRequest<OperationResult<bool>>
    {
        public string StudentMarkId { get; set; }
        public decimal Mark { get; set; }
        public string Comment { get; set; }
        public string LecturerId { get; set; }
    }
}