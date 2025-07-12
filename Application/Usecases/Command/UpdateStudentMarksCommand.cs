using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateStudentMarksCommand : IRequest<OperationResult<bool>>
    {
        public string LecturerId { get; set; }
        public List<InputMark> InputMarks { get; set; }
    }
    public class InputMark
    {
       public string StudentMarkID { get; set; }
       public decimal  Mark { get; set; }
       public string? Comment { get; set; }
    }
}
