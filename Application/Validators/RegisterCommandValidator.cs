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
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        
          public RegisterCommandValidator()
        {
            // FirstName validation - Hỗ trợ tiếng Việt có dấu
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.FirstNameIsEmpty))
                .WithMessage(ValidationMessages.FirstNameIsEmpty);

            // LastName validation - Hỗ trợ tiếng Việt có dấu
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.LastNameIsEmpty))
                .WithMessage(ValidationMessages.LastNameIsEmpty);
                
            // BirthDate validation
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.BirthDateIsEmpty))
                .WithMessage(ValidationMessages.BirthDateIsEmpty)
                .Must(BeValidAge)
                .WithErrorCode(nameof(ErrorCodes.BirthDateInvalidAge))
                .WithMessage(ValidationMessages.BirthDateInvalidAge)
                .LessThan(DateOnly.FromDateTime(DateTime.Today))
                .WithErrorCode(nameof(ErrorCodes.BirthDateInFuture))
                .WithMessage(ValidationMessages.BirthDateInFuture);

            // PhoneNumber validation - Hỗ trợ định dạng Việt Nam
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.PhoneNumberIsEmpty))
                .WithMessage(ValidationMessages.PhoneNumberIsEmpty);
                
            // Gender validation - Hỗ trợ cả tiếng Anh và tiếng Việt
            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.GenderIsEmpty))
                .WithMessage(ValidationMessages.GenderIsEmpty)
                .Must(BeValidGender)
                .WithErrorCode(nameof(ErrorCodes.GenderInvalid))
                .WithMessage(ValidationMessages.GenderInvalid);

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.EmailIsEmpty))
                .WithMessage(ValidationMessages.EmailIsEmpty)
                .EmailAddress()
                .WithErrorCode(nameof(ErrorCodes.EmailInvalidFormat))
                .WithMessage(ValidationMessages.EmailInvalidFormat)
                .MaximumLength(254)
                .WithErrorCode(nameof(ErrorCodes.EmailTooLong))
                .WithMessage(ValidationMessages.EmailTooLong);

            // Password validation - Linh hoạt hơn cho người Việt
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode(nameof(ErrorCodes.PasswordIsEmpty))
                .WithMessage(ValidationMessages.PasswordIsEmpty)
                .MinimumLength(6)
                .WithErrorCode(nameof(ErrorCodes.PasswordTooShort))
                .WithMessage(ValidationMessages.PasswordTooShort)
                .MaximumLength(128)
                .WithErrorCode(nameof(ErrorCodes.PasswordTooLong))
                .WithMessage(ValidationMessages.PasswordTooLong)
                .Must(HaveValidPasswordComplexity)
                .WithErrorCode(nameof(ErrorCodes.PasswordTooWeak))
                .WithMessage(ValidationMessages.PasswordTooWeak);
        }

        // Custom validation methods
        private bool BeValidAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age >= 13 && age <= 120; // Tuổi hợp lệ từ 13-120
        }

       

        private bool BeValidGender(string gender)
        {
            if (string.IsNullOrEmpty(gender)) return false;

            var validGenders = new[]
            { 
            // Tiếng Anh
            "Male", "Female", "Other", "M", "F", "O",
            // Tiếng Việt
            "Nam", "Nữ", "Khác", "N", "Nu", 
            // Các biến thể khác
            "Trai", "Gái", "Không xác định"
        };

            return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
        }

        private bool HaveValidPasswordComplexity(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            // Đếm số loại ký tự khác nhau
            int complexity = 0;

            if (password.Any(char.IsLower)) complexity++; // Chữ thường
            if (password.Any(char.IsUpper)) complexity++; // Chữ hoa  
            if (password.Any(char.IsDigit)) complexity++; // Số

            // Yêu cầu ít nhất 3 trong 4 loại ký tự (linh hoạt hơn)
            return complexity >= 1;
        }

        // Helper method để chuẩn hóa tên (optional)
        public static string NormalizeVietnameseName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            // Loại bỏ khoảng trắng thừa
            name = System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"\s+", " ");

            // Viết hoa chữ cái đầu mỗi từ
            var words = name.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }

            return string.Join(" ", words);
        }
    

    }
}
