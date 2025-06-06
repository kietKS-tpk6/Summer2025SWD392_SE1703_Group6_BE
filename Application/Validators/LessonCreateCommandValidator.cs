using System;
using FluentValidation;
using Application.Common.Constants;
using Application.Usecases.Command;

namespace Application.Validators
{
    public class LessonCreateCommandValidator : AbstractValidator<LessonCreateCommand>
    {
        public LessonCreateCommandValidator()
        {
            RuleFor(x => x.ClassID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.ClassIDIsEmpty))
                .WithMessage(ValidationMessages.ClassIDIsEmpty);

            RuleFor(x => x.SyllabusScheduleID)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.SyllabusScheduleIDIsEmpty)
                .WithMessage(ValidationMessages.SyllabusScheduleIDIsEmpty);

            RuleFor(x => x.LecturerID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.LecturerIDIsEmpty))
                .WithMessage(ValidationMessages.LecturerIDIsEmpty);

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.Now)
                .WithErrorCode(ErrorCodes.LessonStartTimeInvalid)
                .WithMessage(ValidationMessages.LessonStartTimeInvalid);
        }
    }
}
