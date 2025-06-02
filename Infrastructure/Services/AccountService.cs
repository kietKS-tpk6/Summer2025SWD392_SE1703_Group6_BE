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

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private const int AGE_TO_USE = 16;
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
        public async Task<string> Register(RegisterCommand registerCommand)
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
            newAcc.Gender = (Domain.Enums.Gender)Enum.Parse(typeof(Domain.Enums.Gender), registerCommand.Gender, true);
            newAcc.Image = "https://s3.amazonaws.com/37assets/svn/765-default-avatar.png";
            newAcc.Status = Domain.Enums.AccountStatus.Active;
            newAcc.Role = Domain.Enums.AccountRole.Student;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password);
            newAcc.HashPass = hashedPassword;


            var res = await _accountRepository.RegisterAsync(newAcc);
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
                if(res) return true;

                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
