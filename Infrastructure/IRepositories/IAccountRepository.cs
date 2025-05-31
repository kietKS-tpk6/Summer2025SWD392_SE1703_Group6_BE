using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
namespace Infrastructure.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account?> LoginAsync(LoginCommand loginCommand);
    }
}
