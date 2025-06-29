using Application.Common.Constants;
using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateTestEventCommandValidator : AbstractValidator<UpdateTestEventCommand>
    {
        public UpdateTestEventCommandValidator()
        {
            RuleFor(x => x.TestEventIdToUpdate)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.TestEventIDIsEmpty))
                .WithMessage(ValidationMessages.TestEventIDIsEmpty);

            RuleFor(x => x.TestID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.TestIDIsEmpty))
                .WithMessage(ValidationMessages.TestIDIsEmpty);

            RuleFor(x => x.StartAt)
                .LessThan(x => x.EndAt)
                .WithErrorCode(nameof(ErrorCodes.InvalidStartEndTime))
                .WithMessage(ValidationMessages.InvalidStartEndTime);

            RuleFor(x => x.AttemptLimit)
                .GreaterThanOrEqualTo(1)
                .WithErrorCode(nameof(ErrorCodes.AttemptLimitInvalid))
                .WithMessage(ValidationMessages.AttemptLimitInvalid);
        }
    }
}
