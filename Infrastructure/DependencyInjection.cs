using System.ComponentModel.Design;
using System.Reflection;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.BackgroundServices;
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
            //Background service
            services.AddHostedService<ClassStatusBackgroundService>();
            //CommandHandler 
            //Authent
            services.AddScoped<LoginCommandHandler>();
            services.AddScoped<RegisterCommandHandler>();
                //Subject
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();
                //AssessmentCriteria 
            services.AddScoped<AssessmentCriteriaSetupCommandHandler>();
            services.AddScoped<AssessmentCriteriaUpdateCommandHandler>();
            services.AddScoped<SendOTPViaEmailCommandHandler>();
                //Class
            services.AddScoped<ClassCreateCommandHandler>();
            services.AddScoped<ClassUpdateCommandHandler>();
                //Lesson
            services.AddScoped<LessonCreateCommandHandler>();
            services.AddScoped<LessonUpdateCommandHandler>();
            services.AddScoped<LessonCreateFromScheduleCommandHandler>();
                //Attendance 
            services.AddScoped<AttendanceCheckCommandHandler>();
                //TestEvent
            services.AddScoped<UpdateTestEventCommandHandler>();
            //Other
            services.AddScoped<SendOTPViaEmailCommandHandler>();

            //Services 
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IAssessmentCriteriaService, AssessmentCriteriaService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ISyllabusScheduleService, SyllabusScheduleService>();
            services.AddScoped<ISyllabusScheduleTestService, SyllabusScheduleTestService>();
            services.AddScoped<ISystemConfigService, SystemConfigService>();
            services.AddScoped<ITestEventService, TestEventService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<ITestSectionService, TestSectionService>();
            services.AddScoped<IMCQOptionRepository, MCQOptionRepository>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITestSectionService, TestSectionService>();
            services.AddScoped<IMCQAnswerDetailService, MCQAnswerDetailService>();
            services.AddScoped<IMCQAnswerService, MCQAnswerService>();
            services.AddScoped<IWritingAnswerService, WritingAnswerService>();
            services.AddScoped<IStudentTestService, StudentTestService>();
            services.AddScoped<IMCQOptionService, MCQOptionService>();

            services.AddScoped<IAttendanceService, AttendanceService>();
            //Repositories
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAssessmentCriteriaRepository, AssessmentCriteriaRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ISyllabusScheduleRepository, SyllabusScheduleRepository>();
            services.AddScoped<ISyllabusScheduleTestRepository, SyllabusScheduleTestRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
            services.AddScoped<ITestEventRepository, TestEventRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ITestSectionRepository, TestSectionRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<ITestSectionRepository, TestSectionRepository>();
            services.AddScoped<IMCQAnswerDetailRepository, MCQAnswerDetailRepository>();
            services.AddScoped<IMCQAnswerRepository, MCQAnswerRepository>();
            services.AddScoped<IWritingAnswerRepository, WritingAnswerRepository>();
            services.AddScoped<IStudentTestRepository, StudentTestRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IStudentMarkRepository, StudentMarkRepository>();


            //CommandHandler
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<CreateQuestionsCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();
            services.AddScoped<SyllabusScheduleCreateCommand>();
            services.AddScoped<SubmitStudentTestCommandHandler>();


            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped<CreateTestCommandHandler>();
            services.AddScoped<UpdateTestCommandHandler>();
            services.AddScoped<UpdateTestStatusCommandHandler>();
            services.AddScoped<DeleteTestCommandHandler>();

            //CommandHandler
            services.AddScoped<CreateSubjectCommandHandler>();
            services.AddScoped<UpdateSubjectCommandHandler>();
            services.AddScoped<DeleteSubjectCommandHandler>();
            services.AddScoped<ProcessWebhookCommandHandler>();
            services.AddScoped<CreateTestSectionCommandHandler>();
            services.AddScoped<UpdateTestSectionCommandHandler>();
            services.AddScoped<DeleteTestSectionCommandHandler>();

            services.AddMediatR(cfg =>
           cfg.RegisterServicesFromAssembly(Assembly.Load("Application"))
       );

            services.AddHostedService<TestAutoApprovalService>();


            services.Configure<PaymentSettings>(configuration.GetSection("PaymentSettings"));

            return services;
        }
    }
}