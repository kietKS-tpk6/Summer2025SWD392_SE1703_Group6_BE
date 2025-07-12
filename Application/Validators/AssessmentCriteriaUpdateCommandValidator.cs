using FluentValidation;
using Application.Common.Constants;
using Application.Usecases.Command;

public class AssessmentCriteriaUpdateCommandValidator : AbstractValidator<AssessmentCriteriaUpdateCommand>
{
    public AssessmentCriteriaUpdateCommandValidator()
    {
        
        RuleFor(x => x.WeightPercent)
            .NotNull()
            .WithErrorCode(nameof(ErrorCodes.WeightPercentWrongType))
            .WithMessage(ValidationMessages.WeightPercentWrongType)
            .GreaterThan(0)
            .WithErrorCode(nameof(ErrorCodes.WeightPercentInvalid))
            .WithMessage(ValidationMessages.WeightPercentInvalid);

        RuleFor(x => x.RequiredTestCount)
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
        RuleFor(x => x.MinPassingScore)
            .NotNull()
            .WithErrorCode(nameof(ErrorCodes.MinPassingScoreWrongType))
            .WithMessage(ValidationMessages.MinPassingScoreWrongType)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(nameof(ErrorCodes.MinPassingScoreInvalid))
            .WithMessage(ValidationMessages.MinPassingScoreInvalid);
    }
}
