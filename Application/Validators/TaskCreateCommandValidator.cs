using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;
using FluentValidation;

namespace Application.Validators
{
    public class TaskCreateCommandValidator : AbstractValidator<TaskCreateCommand>
    {
        public TaskCreateCommandValidator()
        {
            RuleFor(x => x.LecturerID)
                .NotEmpty()
                .WithMessage("LecturerID không được để trống")
                .MaximumLength(6)
                .WithMessage("LecturerID không được vượt quá 6 ký tự");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Type phải là một giá trị hợp lệ");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content không được để trống")
                .MaximumLength(255)
                .WithMessage("Content không được vượt quá 255 ký tự");

            RuleFor(x => x.DateStart)
                .NotEmpty()
                .WithMessage("DateStart không được để trống");

            RuleFor(x => x.Deadline)
                .NotEmpty()
                .WithMessage("Deadline không được để trống")
                .GreaterThan(x => x.DateStart)
                .WithMessage("Deadline phải sau DateStart");

            RuleFor(x => x.Note)
                .MaximumLength(255)
                .WithMessage("Note không được vượt quá 255 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Note));

            RuleFor(x => x.ResourcesURL)
                .Must(BeAValidUrl)
                .WithMessage("ResourcesURL phải là một URL hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.ResourcesURL));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}