﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public  class ErrorCodes
    {

        
            public const string DataIsEmpty = "DataIsEmpty";
        //LoginCommand

        public const string EmailIsEmpty = "EmailIsEmpty";
            public const string EmailIsNull = "EmailIsNull";
            public const string EmailInvalidFormat = "EmailInvalidFormat";
        public const string NotAllow0 = "notAllow0";

            public const string PasswordIsEmpty = "PasswordIsEmpty";
            public const string PasswordIsNull = "PasswordIsNull";
            public const string PasswordMustBe6Digits = "PasswordMustBe6Digits";
        //Register
            // Email
            public const string EmailTooLong = "EmailTooLong";

            // Password
            public const string PasswordTooShort = "PasswordTooShort";
            public const string PasswordTooLong = "PasswordTooLong";
            public const string PasswordTooWeak = "PasswordTooWeak";

            // FirstName
            public const string FirstNameIsEmpty = "FirstNameIsEmpty";
            public const string FirstNameInvalidLength = "FirstNameInvalidLength";
            public const string FirstNameInvalidFormat = "FirstNameInvalidFormat";

            // LastName
            public const string LastNameIsEmpty = "LastNameIsEmpty";
            public const string LastNameInvalidLength = "LastNameInvalidLength";
            public const string LastNameInvalidFormat = "LastNameInvalidFormat";

            // BirthDate
            public const string BirthDateIsEmpty = "BirthDateIsEmpty";
            public const string BirthDateInvalidAge = "BirthDateInvalidAge";
            public const string BirthDateInFuture = "BirthDateInFuture";

            // PhoneNumber
            public const string PhoneNumberIsEmpty = "PhoneNumberIsEmpty";
            public const string PhoneNumberInvalidFormat = "PhoneNumberInvalidFormat";

            // Gender
            public const string GenderIsEmpty = "GenderIsEmpty";
            public const string GenderInvalid = "GenderInvalid";

            public const string UserNotFound = "UserNotFound";
        //OTP
        public const string OTPIsEmpty = "OTPIsEmpty";

        //Syllabus
        public const string AccountIDIsEmpty = "AccountIDIsEmpty";

        //AssessmentCriteriaCreateCommand
        public const string AssessmentCriteriaIDIsEmpty = "AssessmentCriteriaIDIsEmpty";
        public const string SyllabusIDIsEmpty = "SyllabusIDIsEmpty";
        public const string WeightPercentInvalid = "WeightPercentInvalid";
        public const string CategoryInvalid = "CategoryInvalid";
        public const string RequiredCountInvalid = "RequiredCountInvalid";
        public const string DurationInvalid = "DurationInvalid";
        public const string TestTypeInvalid = "TestTypeInvalid";
        public const string MinPassingScoreInvalid = "MinPassingScoreInvalid";
        public const string WeightPercentWrongType = "WeightPercentWrongType";
        public const string RequiredCountWrongType = "RequiredCountWrongType";
        public const string DurationWrongType = "DurationWrongType";
        public const string MinPassingScoreWrongType = "MinPassingScoreWrongType";
        // ClassCreateCommand
        public const string LecturerIDIsEmpty = "LecturerIDIsEmpty";
        public const string SubjectIDIsEmpty = "SubjectIDIsEmpty";
        public const string ClassNameIsEmpty = "ClassNameIsEmpty";
        public const string PriceOfClassInvalid = "PriceOfClassInvalid";
        public const string TeachingStartTimeInvalid = "TeachingStartTimeInvalid";
        public const string ImageURLIsEmpty = "ImageURLIsEmpty";
        public const string MinStudentAcpInvalid = "MinStudentAcpInvalid";
        public const string MaxStudentAcpInvalid = "MaxStudentAcpInvalid";
        public const string MaxLessThanMin = "MaxLessThanMin";
        public const string ClassIDIsEmpty = "ClassIDIsEmpty";

        //Lesson
        public const string LessonIDIsEmpty = "LessonIDIsEmpty";
        public const string LessonStartTimeInvalid = "LessonStartTimeInvalid";
        public const string SyllabusScheduleIDIsEmpty = "SyllabusScheduleIDIsEmpty";
        public const string ClassLessonIDIsEmpty = "ClassLessonIDIsEmpty";

        //Subject
        public const string SubjectCannotActivate = "SubjectCannotActivate";
        public const string ScheduleIncomplete = "ScheduleIncomplete";
        public const string AssessmentCriteriaIncomplete = "AssessmentCriteriaIncomplete";
        //TestEvent
        public const string TestEventIDIsEmpty = "TestEventIDIsEmpty";
        public const string TestIDIsEmpty = "TestIDIsEmpty";
        public const string InvalidStartEndTime = "InvalidStartEndTime";
        public const string AttemptLimitInvalid = "AttemptLimitInvalid";
        //Check lectures free 
        public const string DateStartInvalid = "DateStartInvalid";
        public const string TimeIsEmpty = "TimeIsEmpty";
        public const string DayOfWeeksIsNull = "DayOfWeeksIsNull";
        public const string DayOfWeeksOutOfRange = "DayOfWeeksOutOfRange";

    }
}
