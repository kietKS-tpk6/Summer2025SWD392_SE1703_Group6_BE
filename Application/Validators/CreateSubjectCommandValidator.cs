using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;
using FluentValidation;

namespace Application.Validators
{
    public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
    {
        public CreateSubjectCommandValidator()
        {
            RuleFor(x => x.SubjectName)
                .NotEmpty()
                 .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
                 .WithMessage(ValidationMessages.DataIsEmpty)
                .MaximumLength(40)
                .WithMessage(ValidationMessages.SubjectNameExceedReqCharacters);
            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(ValidationMessages.exceed255Characters);

            RuleFor(x => x.MinAverageScoreToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessages.ScoreMustBeNonNegative)
                .LessThanOrEqualTo(10)
                .WithMessage(ValidationMessages.ScoreMustNotExceed10);
        }
    }
}