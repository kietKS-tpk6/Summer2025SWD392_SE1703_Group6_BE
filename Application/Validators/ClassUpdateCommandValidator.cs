using System;
using FluentValidation;
using Application.Common.Constants;
using Application.Usecases.Command;

namespace Application.Validators
{
    public class ClassUpdateCommandValidator : AbstractValidator<ClassUpdateCommand>
    {
        public ClassUpdateCommandValidator()
        {
            RuleFor(x => x.ClassID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.ClassIDIsEmpty))
                .WithMessage("Class ID must not be empty.");

            RuleFor(x => x.LecturerID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.LecturerIDIsEmpty))
                .WithMessage(ValidationMessages.LecturerIDIsEmpty);

            RuleFor(x => x.SubjectID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.SubjectIDIsEmpty))
                .WithMessage(ValidationMessages.SubjectIDIsEmpty);

            RuleFor(x => x.ClassName)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.ClassNameIsEmpty))
                .WithMessage(ValidationMessages.ClassNameIsEmpty);

            RuleFor(x => x.MinStudentAcp)
                .GreaterThan(0)
                .WithErrorCode(nameof(ErrorCodes.MinStudentAcpInvalid))
                .WithMessage(ValidationMessages.MinStudentAcpInvalid);

            RuleFor(x => x.MaxStudentAcp)
                .GreaterThan(0)
                .WithErrorCode(nameof(ErrorCodes.MaxStudentAcpInvalid))
                .WithMessage(ValidationMessages.MaxStudentAcpInvalid);

            RuleFor(x => x)
                .Must(x => x.MaxStudentAcp >= x.MinStudentAcp)
                .WithErrorCode(nameof(ErrorCodes.MaxLessThanMin))
                .WithMessage(ValidationMessages.MaxLessThanMin);

            RuleFor(x => x.PriceOfClass)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(nameof(ErrorCodes.PriceOfClassInvalid))
                .WithMessage(ValidationMessages.PriceOfClassInvalid);

            RuleFor(x => x.TeachingStartTime)
                .GreaterThan(DateTime.Now)
                .WithErrorCode(nameof(ErrorCodes.TeachingStartTimeInvalid))
                .WithMessage(ValidationMessages.TeachingStartTimeInvalid);

            RuleFor(x => x.ImageURL)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.ImageURLIsEmpty))
                .WithMessage(ValidationMessages.ImageURLIsEmpty);
        }
    }
}
