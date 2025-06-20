using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TestType
    {
        // Hiển thị trên UI
        // Không xác định kỹ năng cụ thể
        None,
        // Từ vựng
        Vocabulary,
        // Ngữ pháp
        Grammar,
        // Nghe hiểu
        Listening,
        // Đọc hiểu
        Reading,
        // Viết
        Writing,
        // Tổng hợp (ví dụ: đọc + viết)
        Mix,
        //Trắc nghiệm 
        MCQ,
        // Hình thức khác (ghi chú chi tiết nếu cần)
        Other
    }
}
