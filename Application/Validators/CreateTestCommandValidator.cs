using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateTestCommandValidator : AbstractValidator<CreateTestCommand>
    {
        public CreateTestCommandValidator()
        {
            RuleFor(x => x.AccountID)
                .NotEmpty()
                .WithMessage("Account ID is required");

            RuleFor(x => x.SubjectID)
                .NotEmpty()
                .WithMessage("Subject ID is required");

            RuleFor(x => x.TestType)
                .IsInEnum()
                .WithMessage("Invalid test type");

            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Invalid test category");
        }
    }
}