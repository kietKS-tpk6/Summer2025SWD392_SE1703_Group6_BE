﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public static class OperationMessages
    {
        public static string CreateSuccess(string target) =>
            $"Đã thêm {target} thành công.";

        public static string CreateFail(string target) =>
            $"Không thể thêm {target}. Vui lòng thử lại.";

        public static string UpdateSuccess(string target) =>
            $"Đã cập nhật {target} thành công.";

        public static string UpdateFail(string target) =>
            $"Không thể cập nhật {target}. Vui lòng kiểm tra lại.";

        public static string DeleteSuccess(string target) =>
            $"Đã xóa {target} thành công.";

        public static string DeleteFail(string target) =>
            $"Không thể xóa {target}. Vui lòng thử lại hoặc liên hệ hỗ trợ.";

        public static string NotFound(string target) =>
            $"{target} không tồn tại hoặc đã bị xóa.";

        public static string InvalidInput(string target) =>
            $"{target} không hợp lệ. Vui lòng kiểm tra lại thông tin.";

        public static string AlreadyExists(string target) =>
            $"{target} đã tồn tại. Vui lòng chọn giá trị khác.";
        public static string RetrieveSuccess(string target) =>
    $"Đã truy xuất {target} thành công.";

        public static string RetrieveFail(string target) =>
            $"Không thể truy xuất {target}. Vui lòng thử lại sau.";

    }
}

