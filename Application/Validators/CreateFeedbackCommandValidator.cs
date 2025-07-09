using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
    {
        public CreateFeedbackCommandValidator()
        {
            RuleFor(x => x.ClassID)
                .NotEmpty()
                .WithMessage("Class ID is required")
                .MaximumLength(6)
                .WithMessage("Class ID must not exceed 6 characters");

            RuleFor(x => x.StudentID)
                .NotEmpty()
                .WithMessage("Student ID is required")
                .MaximumLength(6)
                .WithMessage("Student ID must not exceed 6 characters");

            RuleFor(x => x.Rating)
                .NotEmpty()
                .WithMessage("Rating is required")
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .MaximumLength(1000)
                .WithMessage("Comment must not exceed 1000 characters");
        }
    }
}