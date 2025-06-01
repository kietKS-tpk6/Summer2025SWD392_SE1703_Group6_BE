using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;
using Infrastructure.Repositories;
using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Application.Usecases.CommandHandler;
using Infrastructure.IRepositories;
namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HangulLearningSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
             //Hash
            //services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
            //CommandHandler 
            services.AddScoped<LoginCommandHandler>();
            services.AddScoped<RegisterCommandHandler>();
            //Services 
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ITokenService, TokenService>();

            //Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            
            return services;
        }
    }

}