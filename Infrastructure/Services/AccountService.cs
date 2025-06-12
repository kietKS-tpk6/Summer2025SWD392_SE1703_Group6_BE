using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using Infrastructure.IRepositories;
using BCrypt.Net;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Application.Common.Shared;
using Infrastructure.Repositories;
using System.Data;
using Application.Common.Constants;

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private const int AGE_TO_USE = 16;
        private const string DEFAULT_PASS = "Hello123";

        private readonly IAccountRepository _accountRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IOTPRepository _OTPRepository;

        public AccountService(IAccountRepository accountRepository, ITokenService tokenService, IEmailService emailService, IOTPRepository oTPRepository)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _OTPRepository = oTPRepository;
        }

        public async Task<LoginDTO> Login(LoginCommand loginCommand)
        {
            var accByEmail = await _accountRepository.GetAccountsByEmailAsync(loginCommand.Email);

            if (accByEmail == null)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            var hashedPasswordFromDb = await _accountRepository.GetHassPassAccountWithEmailAsync(loginCommand);
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginCommand.Password, hashedPasswordFromDb);
            if (isPasswordCorrect == false)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            var claims = new List<Claim>
        {
            new Claim("AccountID", accByEmail.AccountID),
            new Claim("Role", accByEmail.Role.ToString()),
            new Claim("LastName", accByEmail.LastName),
            new Claim("FirstName", accByEmail.FirstName),

        };
            var token = _tokenService.GenerateToken(claims, 4);

            return new LoginDTO { Token = token };
        }

        public async Task<bool> Register(RegisterCommand registerCommand)
        {
            var accByEmail = await _accountRepository.GetAccountsByEmailAsync(registerCommand.Email);
            if (accByEmail != null)
                throw new ArgumentException("Email đã được sử dụng, vui lòng chọn email khác.");

            var accByPhone = await _accountRepository.GetAccountsByPhoneAsync(registerCommand.PhoneNumber);
            if (accByPhone != null)
                throw new ArgumentException("Số điện thoại đã được sử dụng, vui lòng chọn số điện thoại khác.");

            var newAcc = new Account();
            var numberOfAcc = (await _accountRepository.GetNumbeOfAccountsAsync());
            string newAccountId = "A" + numberOfAcc.ToString("D5"); // D5 = 5 chữ số, vd: 00001

            newAcc.AccountID = newAccountId;
            newAcc.BirthDate = registerCommand.BirthDate;
            newAcc.PhoneNumber = registerCommand.PhoneNumber;
            newAcc.Email = registerCommand.Email;
            newAcc.FirstName = registerCommand.FirstName;
            newAcc.LastName = registerCommand.LastName;
            newAcc.Gender = NormalizeGender(registerCommand.Gender).Value;
            newAcc.Image = "https://s3.amazonaws.com/37assets/svn/765-default-avatar.png";
            newAcc.Status = Domain.Enums.AccountStatus.Active;
            newAcc.Role = Domain.Enums.AccountRole.Student;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password);
            newAcc.HashPass = hashedPassword;

            var res = await _accountRepository.CreateAccountAsync(newAcc);
            await _emailService.SendWelcomeEmailAsync(newAcc.Email, newAcc.LastName);
            return res;
        }

        public async Task<bool> VerifyOTPByEmail(VerifyOTPCommand verifyOTPCommand)
        {
            var otp = await _OTPRepository.getOTPViaEmailAndCodeAsync(verifyOTPCommand.Email, verifyOTPCommand.OTP);
            var now = DateTime.Now;

            if (otp.Email.Equals(verifyOTPCommand.Email, StringComparison.OrdinalIgnoreCase) &&
                otp.OTPCode.Equals(verifyOTPCommand.OTP) &&
                otp.ExpirationTime > now)
            {
                var res = await _OTPRepository.UpdateOTPViaOTPCodeAsync(otp.OTPCode);
                if (res) return true;

                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CheckEmailExistAsync(string email)
        {
            var accByEmail = await _accountRepository.GetAccountsByEmailAsync(email);
            if (accByEmail != null)
                return true;
            return false;
        }
        public async Task<string> GetAccountNameByIDAsync(string accountID)
        {
             return await _accountRepository.GetAccountNameByIDAsync(accountID);
            
        }
        public async Task<bool> CheckPhoneExistAsync(string phone)
        {
            var accByPhone = await _accountRepository.GetAccountsByPhoneAsync(phone);
            if (accByPhone != null)
                return true;
            return false;
        }
        public async Task<bool> CreateAccountByManager(CreateAccountCommand createAccountCommand)
        {
         
            var newAcc = new Account();
            var numberOfAcc = (await _accountRepository.GetNumbeOfAccountsAsync());
            string newAccountId = "A" + numberOfAcc.ToString("D5");

            newAcc.AccountID = newAccountId;
            newAcc.BirthDate = createAccountCommand.BirthDate;
            newAcc.PhoneNumber = createAccountCommand.PhoneNumber;
            newAcc.Email = createAccountCommand.Email;
            newAcc.FirstName = createAccountCommand.FirstName;
            newAcc.LastName = createAccountCommand.LastName;
            newAcc.Gender = NormalizeGender(createAccountCommand.Gender).Value;
            newAcc.Image = "https://s3.amazonaws.com/37assets/svn/765-default-avatar.png";
            newAcc.Status = Domain.Enums.AccountStatus.Active;

            // Sử dụng NormalizeRole với isRequired = true (mặc định)
            newAcc.Role = NormalizeRole(createAccountCommand.Role).Value; // .Value vì chắc chắn không null

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(DEFAULT_PASS);
            newAcc.HashPass = hashedPassword;

            var res = await _accountRepository.CreateAccountAsync(newAcc);
            await _emailService.SendWelcomeEmailWithPassAsync(newAcc.Email, newAcc.LastName, DEFAULT_PASS);
            return res;
        }

        public async Task<PagedResult<AccountForManageDTO>> GetPaginatedAccountListAsync(int page, int pageSize, string? role, string? gender, string? status)
        {
            // Chuẩn hóa các tham số search
            var roleToSearch = NormalizeRole(role, isRequired: false);
            var genderToSearch = NormalizeGender(gender, isRequired: false);
            var statusToSearch = NormalizeStatus(status, isRequired: false);

            var (items, total) = await _accountRepository.GetPaginatedAccountListAsync(page, pageSize, roleToSearch, genderToSearch, statusToSearch);
            return new PagedResult<AccountForManageDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        public async Task<OperationResult<bool>> IsLectureFreeAsync(string lecturerId, string subjectId, TimeOnly time, List<DayOfWeek> days)
        {
            return await _accountRepository.IsLectureFreeAsync(lecturerId, subjectId, time, days);
        }

        //public async Task<List<TeachingScheduleDTO>> GetTeachingSchedule()
        //{
        //    return await _accountRepository.GetTeachingSchedule();
        //}
        #region Private Normalization Methods

        /// <summary>
        /// Chuẩn hóa Gender: viết hoa chữ cái đầu và parse thành enum
        /// </summary>
        /// <param name="gender">Gender string cần chuẩn hóa</param>
        /// <param name="isRequired">Có bắt buộc không null hay không</param>
        /// <returns>Gender enum đã được chuẩn hóa hoặc null nếu không bắt buộc</returns>
        private Domain.Enums.Gender? NormalizeGender(string gender, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(gender))
            {
                if (isRequired)
                    throw new ArgumentException("Giới tính không được để trống.");
                return null;
            }

            var normalizedGender = NormalizeString(gender);

            if (normalizedGender == null)
                return null;

            if (Enum.TryParse<Domain.Enums.Gender>(normalizedGender, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Giới tính '{gender}' không hợp lệ. Chỉ chấp nhận: Male, Female, Other.");
        }

        /// <summary>
        /// Chuẩn hóa Role: viết hoa chữ cái đầu và parse thành enum
        /// </summary>
        /// <param name="role">Role string cần chuẩn hóa</param>
        /// <param name="isRequired">Có bắt buộc không null hay không</param>
        /// <returns>AccountRole enum đã được chuẩn hóa hoặc null nếu không bắt buộc</returns>
        private Domain.Enums.AccountRole? NormalizeRole(string role, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                if (isRequired)
                    throw new ArgumentException("Vai trò không được để trống.");
                return null;
            }

            var normalizedRole = NormalizeString(role);

            if (normalizedRole == null)
                return null;

            if (Enum.TryParse<Domain.Enums.AccountRole>(normalizedRole, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Vai trò '{role}' không hợp lệ. Chỉ chấp nhận: Manager, Lecture, Student.");
        }

        /// <summary>
        /// Chuẩn hóa Status: viết hoa chữ cái đầu và parse thành enum
        /// </summary>
        /// <param name="status">Status string cần chuẩn hóa</param>
        /// <param name="isRequired">Có bắt buộc không null hay không</param>
        /// <returns>AccountStatus enum đã được chuẩn hóa hoặc null nếu không bắt buộc</returns>
        private Domain.Enums.AccountStatus? NormalizeStatus(string status, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                if (isRequired)
                    throw new ArgumentException("Trạng thái không được để trống.");
                return null;
            }

            var normalizedStatus = NormalizeString(status);

            if (normalizedStatus == null)
                return null;

            if (Enum.TryParse<Domain.Enums.AccountStatus>(normalizedStatus, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Trạng thái '{status}' không hợp lệ. Chỉ chấp nhận: Active, Inactive, Banned.");
        }

        /// <summary>
        /// Hàm helper để chuẩn hóa string: trim và viết hoa chữ cái đầu
        /// </summary>
        /// <param name="input">String cần chuẩn hóa</param>
        /// <returns>String đã được chuẩn hóa</returns>
        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var normalized = input.Trim();
            if (normalized.Length > 0)
            {
                normalized = char.ToUpper(normalized[0]) + normalized.Substring(1).ToLower();
            }

            return normalized;
        }

        #endregion
    }
}