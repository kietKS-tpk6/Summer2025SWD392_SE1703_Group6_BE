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
    public class SyllabusScheduleCreateCommandValidator : AbstractValidator<SyllabusScheduleCreateCommand>
    {
        public SyllabusScheduleCreateCommandValidator()
        {
            RuleFor(o => o.slotInWeek)
             .NotEmpty()
             .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
                 .WithMessage(ValidationMessages.DataIsEmpty);


            RuleFor(o => o.week)
             .NotEmpty()
             .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
             .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(o => o.subjectID)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);





        }
    }
}
