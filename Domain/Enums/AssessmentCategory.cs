using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum AssessmentCategory
    {
        //Hiển thị trên UI
        // Bài kiểm tra nhỏ
        Quiz,
        // Thuyết trình (Phần [RequiredTestCount] sẽ luôn = 0)
        Presentation,
        // Bài kiểm tra giữa kỳ
        Midterm,
        // Bài kiểm tra cuối kỳ 
        Final,
        // Điểm chuyên cần (Phần [RequiredTestCount] sẽ luôn = 0)
        Attendance,
        //Bài tập (Phần [RequiredTestCount] sẽ luôn = 0)
        Assignment,
        // Đánh giá tham gia hoạt động trong lớp (thảo luận, teamwork)  (Phần [RequiredTestCount] sẽ luôn = 0)
        ClassParticipation,

    }

}
