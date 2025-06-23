using System;
using FluentValidation;
using Application.Common.Constants;
using Application.Usecases.Command;

namespace Application.Validators
{
    public class LessonUpdateCommandValidator : AbstractValidator<LessonUpdateCommand>
    {
        public LessonUpdateCommandValidator()
        {
            RuleFor(x => x.ClassLessonID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.ClassLessonIDIsEmpty))
                .WithMessage(ValidationMessages.ClassLessonIDIsEmpty);

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
