using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
    {
        public CreateSubjectCommandValidator()
        {
            RuleFor(x => x.SubjectID)
                .NotEmpty()
                .WithMessage("Subject ID is required")
                .MaximumLength(6)
                .WithMessage("Subject ID cannot exceed 6 characters")
                .Matches(@"^SJ\d{4}$")
                .WithMessage("Subject ID must follow format SJxxxx (e.g., SJ0001)");

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