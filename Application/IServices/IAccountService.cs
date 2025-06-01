using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IAccountService
    {
        public Task<LoginDTO> Login(LoginCommand loginCommand);

        public Task<RegisterDTO> Register(RegisterCommand registerCommand);
        

        
    }
}
