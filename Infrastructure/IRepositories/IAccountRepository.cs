using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
namespace Infrastructure.IRepositories
{
    internal interface IAccountRepository
    {
        Task<bool> AddAsync(Account account);
    }
}
