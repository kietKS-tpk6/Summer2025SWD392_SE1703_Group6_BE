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
    public class ClassCreateCommandValidator : AbstractValidator<ClassCreateCommand>
    {
        public ClassCreateCommandValidator()
        {
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
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(nameof(ErrorCodes.MinStudentAcpInvalid))
                .WithMessage(ValidationMessages.MinStudentAcpInvalid);

            RuleFor(x => x.MaxStudentAcp)
                .GreaterThanOrEqualTo(0)
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
