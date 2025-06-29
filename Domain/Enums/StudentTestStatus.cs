using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum StudentTestStatus
    {
        Pending,             // Chưa làm
        Submitted,           // Đã nộp bài
        AutoGraded,          // Đã chấm tự động (ví dụ trắc nghiệm)
        WaitingForGrading,
        Graded,              // Đã chấm (bao gồm tự động và thủ công)
        Published            // Đã công bố điểm
    }
}
