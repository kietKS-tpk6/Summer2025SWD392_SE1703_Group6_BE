using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public  class ErrorCodes
    {
            public const string EmailIsEmpty = "EmailIsEmpty";
            public const string EmailIsNull = "EmailIsNull";
            public const string PasswordIsEmpty = "PasswordIsEmpty";
            public const string PasswordIsNull = "PasswordIsNull";
            public const string UserNotFound = "UserNotFound";
        //AssessmentCriteriaCreateCommand
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

    }
}
