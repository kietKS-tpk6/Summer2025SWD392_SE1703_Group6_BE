using System;
using FluentValidation;
using Application.Usecases.Command;
using Application.Common.Constants;

namespace Application.Validators
{
    public class CheckLecturerFreeCommandValidator : AbstractValidator<CheckLecturerFreeCommand>
    {
        public CheckLecturerFreeCommandValidator()
        {
            RuleFor(x => x.SubjectID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.SubjectIDIsEmpty))
                .WithMessage(ValidationMessages.SubjectIDIsEmpty);

            RuleFor(x => x.DateStart)
                .GreaterThan(DateTime.MinValue)
                .WithErrorCode(nameof(ErrorCodes.DateStartInvalid))
                .WithMessage(ValidationMessages.DateStartInvalid);

            RuleFor(x => x.Time)
                .NotNull()
                .WithErrorCode(nameof(ErrorCodes.TimeIsEmpty))
                .WithMessage(ValidationMessages.TimeIsEmpty);

            RuleFor(x => x.dayOfWeeks)
                .NotNull()
                .WithErrorCode(nameof(ErrorCodes.DayOfWeeksIsNull))
                .WithMessage(ValidationMessages.DayOfWeeksIsNull);

            RuleFor(x => x.dayOfWeeks)
                .Must(dows => dows.All(d => d >= 0 && d <= 6))
                .WithErrorCode(nameof(ErrorCodes.DayOfWeeksOutOfRange))
                .WithMessage(ValidationMessages.DayOfWeeksOutOfRange);
        }
    }
}
