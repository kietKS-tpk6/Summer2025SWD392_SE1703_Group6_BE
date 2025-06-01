using Application.IServices;
using Application.Usecases.CommandHandler;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<AssessmentCriteriaCreateHandler>();
            services.AddScoped<SendOTPViaEmailCommandHandler>();

            //Services 
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IAssessmentCriteriaService, AssessmentCriteriaService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISyllabusesService, SyllabusesService>();
            services.AddScoped<IEmailService, EmailService>();

            //Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISyllabusesRepository, SyllabusesRepository>();
            services.AddScoped<IAssessmentCriteriaRepository, AssessmentCriteriaRepository>();
            return services;
        }
    }

}