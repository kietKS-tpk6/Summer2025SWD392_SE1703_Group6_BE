using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class CreateQuestionsCommandValidator : AbstractValidator<CreateQuestionsCommand>
    {
        public CreateQuestionsCommandValidator()
        {
            RuleFor(x => x.NumberOfQuestions)
                .GreaterThan(0)
                .WithMessage("Số lượng câu hỏi phải lớn hơn 0.");

            RuleFor(x => x.Score)
                .GreaterThan(0)
                .WithMessage("Điểm phải lớn hơn 0.");
        }
    }
}
