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
            RuleFor(o => o.slotInWeek)
            .NotEqual(0)
             .WithErrorCode(nameof(ErrorCodes.NotAllow0))
            .WithMessage(ValidationMessages.NotAllow0);


            RuleFor(o => o.week)
                .NotEmpty()
             .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
             .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(o => o.week)
             .NotEqual(0)
             .WithErrorCode(nameof(ErrorCodes.NotAllow0))
            .WithMessage(ValidationMessages.NotAllow0);


            RuleFor(o => o.subjectID)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);





        }
    }
}
