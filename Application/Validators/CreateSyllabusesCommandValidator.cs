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
    public class CreateSyllabusesCommandValidator : AbstractValidator<CreateSyllabusesCommand>
    {
        public CreateSyllabusesCommandValidator()
        {
            RuleFor(o => o.SubjectID)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
                 .WithMessage(ValidationMessages.DataIsEmpty);

            RuleFor(o => o.AccountID)
             .NotEmpty()
             .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
                 .WithMessage(ValidationMessages.DataIsEmpty);

        }
    }
}
