using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ClassStatus
    {
        //Đang tạo, chờ xử lý 
        Pending,
        //Mở tuyển sinh
        Open,
        //Đang dạy 
        Ongoing,
        //Hoàn thành
        Completed,
        //Xóa
        Deleted,
        //Lớp không đủ điều kiện mở.
        Cancelled
    }
}
