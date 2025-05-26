using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
namespace Infrastructure.Data
{
    public class HangulLearningSystemDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}
