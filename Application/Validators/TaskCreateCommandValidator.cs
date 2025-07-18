﻿using Application.Usecases.Command;
using FluentValidation;
using Domain.Enums;
using Application.IServices;

namespace Application.Validators
{
    public class TaskCreateCommandValidator : AbstractValidator<TaskCreateCommand>
    {
        private readonly IAccountService _accountService;

        public TaskCreateCommandValidator(IAccountService accountService)
        {
            _accountService = accountService;

            RuleFor(x => x.LecturerID)
                .NotEmpty()
                .WithMessage("LecturerID không được để trống")
                .MaximumLength(6)
                .WithMessage("LecturerID không được vượt quá 6 ký tự")
                .MustAsync(async (lecturerId, cancellation) =>
                 {
                     var result = await _accountService.GetListAccountByRoleAsync(AccountRole.Lecture);

                     if (!result.Success || result.Data == null)
                         return false;

                     return result.Data.Any(a => a.AccountID == lecturerId);
                 })
                .WithMessage("LecturerID phải thuộc về tài khoản có role là Lecture và đang hoạt động");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Type phải là một giá trị hợp lệ");

            RuleFor(x => x.Deadline)
                .NotEmpty()
                .WithMessage("Deadline không được để trống");

            RuleFor(x => x.Content)
                .MaximumLength(255)
                .WithMessage("Content không được vượt quá 255 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Content));

            RuleFor(x => x.Note)
                .MaximumLength(255)
                .WithMessage("Note không được vượt quá 255 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Note));

            RuleFor(x => x.DateStart)
                .LessThan(x => x.Deadline)
                .WithMessage("DateStart phải trước Deadline")
                .When(x => x.DateStart != default(DateTime));

            RuleFor(x => x.ResourcesURL)
                .NotEmpty()
                .WithMessage("ResourcesURL không được để trống khi TaskType là Meeting")
                .When(x => x.Type == TaskType.Meeting);

            RuleFor(x => x.ResourcesURL)
                .Must(BeAValidUrl)
                .WithMessage("ResourcesURL phải là một URL hợp lệ")
                .When(x => x.Type == TaskType.Meeting && !string.IsNullOrEmpty(x.ResourcesURL));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}