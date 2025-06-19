using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
    {
        public UpdateSubjectCommandValidator()
        {
            RuleFor(x => x.SubjectID)
                .NotEmpty()
                .WithMessage("Subject ID is required")
                .MaximumLength(6)
                .WithMessage("Subject ID cannot exceed 6 characters");

            RuleFor(x => x.SubjectName)
                .NotEmpty()
                .WithMessage("Subject Name is required")
                .MaximumLength(40)
                .WithMessage("Subject Name cannot exceed 40 characters");

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage("Description cannot exceed 255 characters");

            RuleFor(x => x.MinAverageScoreToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum average score must be >= 0")
                .LessThanOrEqualTo(10)
                .WithMessage("Minimum average score must be <= 10");
        }
    }
}