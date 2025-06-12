using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class ClassCreateCommand : IRequest<OperationResult<bool>>
    {
        public string LecturerID { get; set; }
        public string SubjectID { get; set; }
        public string ClassName { get; set; }
        public int MinStudentAcp { get; set; }
        public int MaxStudentAcp { get; set; }
        public decimal PriceOfClass { get; set; }
        public DateTime TeachingStartTime { get; set; }
        public string ImageURL { get; set; }
    }
}
