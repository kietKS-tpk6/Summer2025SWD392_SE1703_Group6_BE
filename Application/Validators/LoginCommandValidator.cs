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
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(o => o.Email)
     .NotEmpty()
     .WithErrorCode(nameof(ErrorCodes.EmailIsEmpty))
     .WithMessage(ValidationMessages.EmailIsEmpty)
     .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
     .WithErrorCode(nameof(ErrorCodes.EmailInvalidFormat))
     .WithMessage(ValidationMessages.EmailInvalidFormat);


            //RuleFor(o => o.Password)
            //    .NotNull()
            //    .WithErrorCode(nameof(ErrorCodes.PasswordIsEmpty))
            //    .WithMessage(ValidationMessages.PasswordIsEmpty);

            RuleFor(o => o.Password)
    .NotEmpty()
    .WithErrorCode(nameof(ErrorCodes.PasswordIsEmpty))
    .WithMessage(ValidationMessages.PasswordIsEmpty);
    //.Matches(@"^\d{6}$") // Regex: đúng 6 chữ số
    //.WithErrorCode(nameof(ErrorCodes.PasswordMustBe6Digits))
    //.WithMessage("Mật khẩu phải gồm đúng 6 chữ số.");

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
