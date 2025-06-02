using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;
using FluentValidation;

namespace Application.Validators
{
    public class VerifyOTPCommandValidator : AbstractValidator<VerifyOTPCommand>
    {
        public VerifyOTPCommandValidator()
        {
            // PhoneNumber validation - Hỗ trợ định dạng Việt Nam
            RuleFor(x => x.OTP)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.OTPIsEmpty))
            .WithMessage(ValidationMessages.OTPIsEmpty);
           
        }
    }
}
