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
    public class UpdateSyllabusesCommandValidator : AbstractValidator<UpdateSyllabusesCommand>
    {
        public UpdateSyllabusesCommandValidator()
        {
            
               
            RuleFor(x => x.Note)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
                .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(x => x.Status)
             .NotEmpty()
             .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
             .WithMessage(ValidationMessages.DataIsEmpty);
        }

    }
}
