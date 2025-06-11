using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.AccountID)
                .NotEmpty()
                .WithMessage("Account ID is required")
                .MaximumLength(6)
                .WithMessage("Account ID cannot exceed 6 characters");

            RuleFor(x => x.ClassID)
                .NotEmpty()
                .WithMessage("Class ID is required")
                .MaximumLength(6)
                .WithMessage("Class ID cannot exceed 6 characters");

            RuleFor(x => x.Description)
                .MaximumLength(200)
                .WithMessage("Description cannot exceed 200 characters");
        }
    }
}