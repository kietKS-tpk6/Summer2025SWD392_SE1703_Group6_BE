using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
    {
        public CreateEnrollmentCommandValidator()
        {
            RuleFor(x => x.PaymentID)
                .NotEmpty()
                .WithMessage("Payment ID is required");

            RuleFor(x => x.StudentID)
                .NotEmpty()
                .WithMessage("Student ID is required");

            RuleFor(x => x.ClassID)
                .NotEmpty()
                .WithMessage("Class ID is required");
        }
    }
}