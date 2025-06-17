using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TestEventStatus
    {
        //Hiển thị trên UI
        //Bản nháp (này là TestID = null)
        Draft,
        //Chưa bắt đầu (thời gian chưa tới)
        NotStarted,
        //Đang diễn ra  
        Ongoing,
        //Đã kết thúc
        Ended,
        //Không hoạt động
        Deleted
    }
}
