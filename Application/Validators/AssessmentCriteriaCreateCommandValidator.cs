using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class AssessmentCriteriaCreateCommandValidator : AbstractValidator<AssessmentCriteriaCreateCommand>
    {
        public AssessmentCriteriaCreateCommandValidator()
        {
            RuleFor(x => x.SyllabusID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.SyllabusIDIsEmpty))
                .WithMessage(ValidationMessages.SyllabusIDIsEmpty);

            RuleFor(x => x.WeightPercent)
                .NotNull()
                .WithErrorCode(nameof(ErrorCodes.WeightPercentWrongType))
                .WithMessage(ValidationMessages.WeightPercentWrongType)
                .GreaterThan(0)
                .WithErrorCode(nameof(ErrorCodes.WeightPercentInvalid))
                .WithMessage(ValidationMessages.WeightPercentInvalid);

            RuleFor(x => x.RequiredCount)
                .NotNull()
                .WithErrorCode(nameof(ErrorCodes.RequiredCountWrongType))
                .WithMessage(ValidationMessages.RequiredCountWrongType)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(nameof(ErrorCodes.RequiredCountInvalid))
                .WithMessage(ValidationMessages.RequiredCountInvalid);


            RuleFor(x => x.Category)
                .IsInEnum()
                .WithErrorCode(nameof(ErrorCodes.CategoryInvalid))
                .WithMessage(ValidationMessages.CategoryInvalid);


            RuleFor(x => x.Duration)
                .NotNull()
                .WithErrorCode(nameof(ErrorCodes.DurationWrongType))
                .WithMessage(ValidationMessages.DurationWrongType)
                .GreaterThan(0)
                .WithErrorCode(nameof(ErrorCodes.DurationInvalid))
                .WithMessage(ValidationMessages.DurationInvalid);

            RuleFor(x => x.TestType)
                .IsInEnum()
                .WithErrorCode(nameof(ErrorCodes.TestTypeInvalid))
                .WithMessage(ValidationMessages.TestTypeInvalid);
            RuleFor(x => x.MinPassingScore)
               .NotNull()
               .WithErrorCode(nameof(ErrorCodes.MinPassingScoreWrongType))
               .WithMessage(ValidationMessages.MinPassingScoreWrongType)
               .GreaterThanOrEqualTo(0)
               .WithErrorCode(nameof(ErrorCodes.MinPassingScoreInvalid))
               .WithMessage(ValidationMessages.MinPassingScoreInvalid);






        }
    }

}
