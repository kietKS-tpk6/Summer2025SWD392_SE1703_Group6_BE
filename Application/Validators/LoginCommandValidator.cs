using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Login;
using FluentValidation;
namespace Application.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(o => o.Email)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.EmailIsEmpty))
                .WithMessage(ValidationMessages.EmailIsEmpty);

            //RuleFor(o => o.Password)
            //    .NotNull()
            //    .WithErrorCode(nameof(ErrorCodes.EmailIsNull))
            //    .WithMessage(ValidationMessages.EmailIsNull);

            //RuleFor(o => o.Password)
            //    .NotEmpty()
            //    .WithErrorCode(nameof(ErrorCodes.PasswordIsEmpty))
            //    .WithMessage(ValidationMessages.PasswordIsEmpty);

            //RuleFor(o => o.Password)
            //    .NotNull()
            //    .WithErrorCode(nameof(ErrorCodes.PasswordIsNull))
            //    .WithMessage(ValidationMessages.PasswordIsNull);
        }
    }

}
