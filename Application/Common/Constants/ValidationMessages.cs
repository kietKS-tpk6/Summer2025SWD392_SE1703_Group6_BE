﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public static class ValidationMessages
    {

        public const string DataIsEmpty = "Dữ liệu {PropertyName} truyền vào bị trống.";
        public const string WrongDataType = "Dữ liệu {PropertyName} không đúng kiểu.";
        public const string SubjectNameExceedReqCharacters = "Dữ liệu {PropertyName} vượt quá 40 ký tự.";
        public const string ScoreMustBeNonNegative = "Dữ liệu {PropertyName} phải lớn hơn hoặc bằng 0.";
        public const string ScoreMustNotExceed10 = "Dữ liệu {PropertyName} phải bé hơn hoặc bằng 10.";
        public const string NotAllow0 = "Dữ liệu {PropertyName} không được phép bằng 0.";

        public const string exceed255Characters = "Dữ liệu {PropertyName} vượt quá 255 ký tự.";
        // Email
        public const string AccountIDIsEmpty = "AccountID không được để trống.";
        public const string EmailIsEmpty = "Email không được để trống.";
        public const string EmailInvalidFormat = "Email không đúng định dạng.";
        public const string EmailTooLong = "Email không được vượt quá 254 ký tự.";
        

        // Password
        public const string PasswordIsEmpty = "Mật khẩu không được để trống.";
        public const string PasswordMustBe6Digits = "Mật khẩu phải có 6 ký tự.";
        public const string PasswordTooLong = "Mật khẩu không được vượt quá 128 ký tự.";
        public const string PasswordTooShort = "Mật khẩu phải có ít nhất 6 ký tự.";
        public const string PasswordTooWeak = "Mật khẩu phải chứa ít nhất 3 trong 4 loại: chữ thường, chữ hoa, số, ký tự đặc biệt.";

        // FirstName
        public const string FirstNameIsEmpty = "Tên không được để trống.";
        public const string FirstNameInvalidLength = "Tên phải có từ 2-50 ký tự.";
        public const string FirstNameInvalidFormat = "Tên chỉ được chứa chữ cái và khoảng trắng.";

        // LastName
        public const string LastNameIsEmpty = "Họ không được để trống.";
        public const string LastNameInvalidLength = "Họ phải có từ 2-50 ký tự.";
        public const string LastNameInvalidFormat = "Họ chỉ được chứa chữ cái và khoảng trắng.";

        // BirthDate
        public const string BirthDateIsEmpty = "Ngày sinh không được để trống.";
        public const string BirthDateInvalidAge = "Tuổi phải từ 13-120.";
        public const string BirthDateInFuture = "Ngày sinh không được ở tương lai.";

        // PhoneNumber
        public const string PhoneNumberIsEmpty = "Số điện thoại không được để trống.";
        public const string PhoneNumberInvalidFormat = "Số điện thoại không đúng định dạng Việt Nam (VD: 0901234567, +84901234567).";

        // Gender
        public const string GenderIsEmpty = "Giới tính không được để trống.";
        public const string GenderInvalid = "Giới tính hợp lệ: Nam/Nữ/Khác, Male/Female/Other.";

        // OTP
        public const string OTPIsEmpty = "OTP không được để trống.";

        // AssessmentCriteria
        public const string AssessmentCriteriaIDIsEmpty = "AssessmentCriteriaID không được để trống.";
        public const string SyllabusIDIsEmpty = "SyllabusID không được để trống.";
        public const string WeightPercentInvalid = "WeightPercent phải lớn hơn 0.";
        public const string WeightPercentWrongType = "WeightPercent phải là số hợp lệ.";
        public const string RequiredCountInvalid = "RequiredCount phải lớn hơn hoặc bằng 0.";
        public const string RequiredCountWrongType = "RequiredCount phải là số nguyên hợp lệ.";
        public const string DurationInvalid = "Duration phải lớn hơn 0.";
        public const string DurationWrongType = "Duration phải là số nguyên hợp lệ.";
        public const string TestTypeInvalid = "TestType không hợp lệ.";
        public const string MinPassingScoreInvalid = "MinPassingScore phải lớn hơn hoặc bằng 0.";
        public const string MinPassingScoreWrongType = "MinPassingScore phải là số hợp lệ.";
        public const string CategoryInvalid = "Category không hợp lệ.";

        // Class
        public const string LecturerIDIsEmpty = "LecturerID không được để trống.";
        public const string SubjectIDIsEmpty = "SubjectID không được để trống.";
        public const string ClassNameIsEmpty = "Tên lớp không được để trống.";
        public const string PriceOfClassInvalid = "Học phí phải lớn hơn hoặc bằng 0.";
        public const string TeachingStartTimeInvalid = "Thời gian bắt đầu dạy phải lớn hơn hiện tại.";
        public const string ImageURLIsEmpty = "Đường dẫn hình ảnh không được để trống.";
        public const string MinStudentAcpInvalid = "Số sinh viên tối thiểu phải lớn hơn 0.";
        public const string MaxStudentAcpInvalid = "Số sinh viên tối đa phải lớn hơn 0.";
        public const string MaxLessThanMin = "Số sinh viên tối đa phải lớn hơn hoặc bằng số sinh viên tối thiểu.";
        public const string ClassIDIsEmpty = "ClassID không được để trống.";
        //Lesson
        public const string ClassLessonIDIsEmpty = "ClassLessonIDIsEmpty";
        public const string SyllabusScheduleIDIsEmpty = "SyllabusScheduleID không được để trống.";
        public const string LessonStartTimeInvalid = "Thời gian bắt đầu buổi học phải lớn hơn thời điểm hiện tại.";
        // General
        public const string UserNotFound = "Người dùng không tồn tại.";

        // Subject
        public const string SubjectCannotActivate = "Không thể kích hoạt môn học này do thiếu thông tin bắt buộc.";
        public const string ScheduleIncomplete = "Môn học chưa có đầy đủ lịch học.";
        public const string AssessmentCriteriaIncomplete = "Môn học chưa có đầy đủ tiêu chí đánh giá.";
        //Test event 
        public const string TestEventIDIsEmpty = "Mã buổi kiểm tra không được để trống.";
        public const string TestIDIsEmpty = "Mã đề thi không được để trống.";
        public const string InvalidStartEndTime = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.";
        public const string AttemptLimitInvalid = "Số lần làm bài phải lớn hơn hoặc bằng 1.";
        //Check lectures free
        public const string DateStartInvalid = "Ngày bắt đầu phải hợp lệ.";
        public const string TimeIsEmpty = "Thời gian không được để trống.";
        public const string DayOfWeeksIsNull = "Danh sách ngày học không được để trống.";
        public const string DayOfWeeksOutOfRange = "Giá trị trong danh sách ngày học phải từ 0 đến 6.";

    }
}

