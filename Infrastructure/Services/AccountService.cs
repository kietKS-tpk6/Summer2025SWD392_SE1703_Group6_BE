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

namespace Infrastructure.Services
{
    public class AccountService: IAccountService
    {
        private const int AGE_TO_USE = 16;
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenService _tokenService;

        public AccountService(IAccountRepository accountRepository, ITokenService tokenService)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginDTO> Login(LoginCommand loginCommand)
        {
            var accByEmail = await _accountRepository.GetAccountsByEmailAsync(loginCommand.Email);

            if (accByEmail == null)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            var hashedPasswordFromDb = await _accountRepository.GetHassPassAccountWithEmailAsync(loginCommand);
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginCommand.Password, hashedPasswordFromDb);
            if (isPasswordCorrect==false)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

             var claims = new List<Claim>
        {
            new Claim("AccountID", accByEmail.AccountID),
            new Claim("Role", accByEmail.Role.ToString()),
            new Claim("LastName", accByEmail.LastName),
            new Claim("FirstName", accByEmail.FirstName),

        };
            var token = _tokenService.GenerateToken(claims, 4);
          

            return new LoginDTO { Token= token };
        }
        public async Task<string> Register(RegisterCommand registerCommand)
        {
            var accByEmail = await _accountRepository.GetAccountsByEmailAsync(registerCommand.Email);
            if(accByEmail != null) 
                throw new ArgumentException("Email đã được sử dụng, vui lòng chọn email khác.");
            var accByPhone = await _accountRepository.GetAccountsByPhoneAsync(registerCommand.PhoneNumber);
            if (accByPhone != null) 
                throw new ArgumentException("Số điện thoại đã được sử dụng, vui lòng chọn số điện thoại khác.");
            if (!Enum.TryParse<Gender>(registerCommand.Gender, ignoreCase: true, out var gender))
                throw new ArgumentException("Giới tính không hợp lệ.");
            if (string.IsNullOrWhiteSpace(registerCommand.FirstName))
                throw new ArgumentException("Vui lòng nhập họ.");
            if (string.IsNullOrWhiteSpace(registerCommand.LastName))
                throw new ArgumentException("Vui lòng nhập tên.");

            //kiểm tra tuổi
            if (registerCommand.BirthDate == default(DateOnly)) // default = 0001-01-01
            {
                throw new ArgumentException("Ngày sinh không hợp lệ.");
            }
            else
            {
                // Lấy ngày hiện tại trừ đi số tuổi tối thiểu
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var todayMinus16 = today.AddYears(-AGE_TO_USE);

                if (registerCommand.BirthDate > todayMinus16)
                {
                    throw new ArgumentException($"Tài khoản phải từ {AGE_TO_USE} tuổi trở lên.");
                }
            }           
            var newAcc = new Account();

            var numberOfAcc = (await _accountRepository.GetNumbeOfAccountsAsync());
            string newAccountId = "A" + numberOfAcc.ToString("D5"); // D5 = 5 chữ số, vd: 00001
            newAcc.AccountID = newAccountId;

            newAcc.BirthDate = registerCommand.BirthDate;
            newAcc.PhoneNumber = registerCommand.PhoneNumber;
            newAcc.Email = registerCommand.Email;
            newAcc.FirstName = registerCommand.FirstName;
            newAcc.LastName = registerCommand.LastName;
            newAcc.Gender = (Domain.Enums.Gender)Enum.Parse(typeof(Domain.Enums.Gender), registerCommand.Gender);
            newAcc.Image = "https://s3.amazonaws.com/37assets/svn/765-default-avatar.png";
            newAcc.Status = Domain.Enums.AccountStatus.Active;
            newAcc.Role = Domain.Enums.AccountRole.Student;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password);
            newAcc.HashPass = hashedPassword;
  

            var res = await _accountRepository.RegisterAsync(newAcc);
            return res;
        }
    
    }
}
