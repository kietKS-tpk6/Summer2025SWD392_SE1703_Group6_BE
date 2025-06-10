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
    public class UpdateTestSchedulesToSlotsCommandValidator : AbstractValidator<UpdateTestSchedulesToSlotsCommand>
    {
        public UpdateTestSchedulesToSlotsCommandValidator()
        {
            RuleFor(x => x.TestType)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(x => x.TestCategory)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(x => x.SyllabusScheduleID)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);
            RuleFor(x => x.syllabusId)
            .NotEmpty()
            .WithErrorCode(nameof(ErrorCodes.DataIsEmpty))
            .WithMessage(ValidationMessages.DataIsEmpty);
        }
    }
}
