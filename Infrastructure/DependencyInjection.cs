using Application.IServices;
using Application.Usecases.Command;
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
            //CommandHandler 
                //Authent
            services.AddScoped<LoginCommandHandler>();
            services.AddScoped<RegisterCommandHandler>();
                //Subject
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();
                //AssessmentCriteria 
            services.AddScoped<AssessmentCriteriaCreateCommandHandler>();
            services.AddScoped<AssessmentCriteriaUpdateCommandHandler>();
            services.AddScoped<SendOTPViaEmailCommandHandler>();
                //Class
            services.AddScoped<ClassCreateCommandHandler>();
            services.AddScoped<ClassUpdateCommandHandler>();
                //Lesson
            services.AddScoped<LessonCreateCommandHandler>();
            services.AddScoped<LessonUpdateCommandHandler>();
            services.AddScoped<LessonCreateFromScheduleCommandHandler>();
            //Other
            services.AddScoped<SendOTPViaEmailCommandHandler>();
            services.AddScoped<CreateSyllabusesCommandHandler>();
            services.AddScoped<SyllabusScheduleCreateCommandHandler>();

            //Services 
            services.AddScoped<ILessonService, LessonService>();


            // Services - Business logic cho Read operations
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IAssessmentCriteriaService, AssessmentCriteriaService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISyllabusesService, SyllabusesService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ISyllabusScheduleService, SyllabusScheduleService>();
            //Repositories
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISyllabusesRepository, SyllabusesRepository>();
            services.AddScoped<IAssessmentCriteriaRepository, AssessmentCriteriaRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ISyllabusScheduleRepository, SyllabusScheduleRepository>();
          

            services.AddScoped<ISyllabusScheduleRepository, SyllabusScheduleRepository>();


            //CommandHandler
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();
            services.AddScoped<SyllabusScheduleCreateCommand>();
            
                            
            return services;
        }
    }
}