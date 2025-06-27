using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class AttendanceCheckCommand : IRequest<OperationResult<bool>>
    {
        public string LessonId { get; set; }
        public List<StudentCheckAttendance> AttendanceRecords { get; set; }
    }
    public class StudentCheckAttendance
    {
        public string AttendanceRecordID { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public string? Note { get; set; }
    }
}
