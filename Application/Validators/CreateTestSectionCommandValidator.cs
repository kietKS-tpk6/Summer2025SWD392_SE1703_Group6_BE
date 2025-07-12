using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateTestSectionCommandValidator : AbstractValidator<CreateTestSectionCommand>
    {
        public CreateTestSectionCommandValidator()
        {
            RuleFor(x => x.TestID)
                .NotEmpty()
                .WithMessage("Test ID is required");

            RuleFor(x => x.Context)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Context is required and cannot exceed 255 characters");

            RuleFor(x => x.TestSectionType)
                .IsInEnum()
                .WithMessage("Invalid test section type");

            RuleFor(x => x.Score)
                .GreaterThan(0)
                .When(x => x.Score.HasValue)
                .WithMessage("Score must be greater than 0 when provided");
        }
    }
}