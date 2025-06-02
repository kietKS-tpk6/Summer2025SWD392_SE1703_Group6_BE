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
            // Database Context
            services.AddDbContext<HangulLearningSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
             //Hash
            //services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
            //CommandHandler 
            services.AddScoped<LoginCommandHandler>();
            services.AddScoped<RegisterCommandHandler>();
                //AssessmentCriteria
            services.AddScoped<AssessmentCriteriaCreateCommandHandler>();
            services.AddScoped<AssessmentCriteriaUpdateCommandHandler>();
            services.AddScoped<SendOTPViaEmailCommandHandler>();
            services.AddScoped<ClassCreateCommandHandler>();
            //Services 
               

            // Services - Business logic cho Read operations
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IAssessmentCriteriaService, AssessmentCriteriaService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISyllabusesService, SyllabusesService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IClassService, ClassService>();
            //Repositories
            services.AddScoped<ISubjectService, SubjectService>(); 

            // Repositories - Data access layer
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISyllabusesRepository, SyllabusesRepository>();
            services.AddScoped<IAssessmentCriteriaRepository, AssessmentCriteriaRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();

            //CommandHandler
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();

            return services;
        }
    }
}