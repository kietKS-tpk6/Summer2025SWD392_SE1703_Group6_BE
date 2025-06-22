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
        None, //0
        // Từ vựng
        Vocabulary, //1
        // Ngữ pháp
        Grammar, //2
        // Nghe hiểu
        Listening, //3
        // Đọc hiểu
        Reading, //4
        // Viết
        Writing, //5
        // Tổng hợp (ví dụ: đọc + viết)
        Mix, //6
        //Trắc nghiệm 
        MCQ, //7
        // Hình thức khác (ghi chú chi tiết nếu cần)
        Other //8
    }
}
