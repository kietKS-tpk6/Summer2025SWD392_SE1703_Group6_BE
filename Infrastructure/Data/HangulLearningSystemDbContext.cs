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
        public HangulLearningSystemDbContext(DbContextOptions<HangulLearningSystemDbContext> options)
      : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.Role)
                .HasConversion<string>();
            modelBuilder.Entity<Account>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Account>()
                .Property(a => a.Gender)
                .HasConversion<string>();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AssessmentCriteria>()
                .Property(a => a.Category)
                .HasConversion<string>();
            modelBuilder.Entity<AttendanceRecord>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Class>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<ClassEnrollment>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Payment>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Question>()
                .Property(a => a.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Question>()
                  .Property(a => a.Score)
                  .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<StudentTest>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Test>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Test>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<TestEvent>()
                .Property(a => a.TestType)
                .HasConversion<string>();
            modelBuilder.Entity<TestEvent>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<SyllabusScheduleTest>()
                .Property(a => a.TestType)
                .HasConversion<string>();
            modelBuilder.Entity<SyllabusScheduleTest>()
              .Property(s => s.IsActive)
                 .HasDefaultValue(true);
            modelBuilder.Entity<Subject>()
                .Property(a => a.Status)
                .HasConversion<string>();
            modelBuilder.Entity<TestSection>()
               .Property(a => a.TestSectionType)
               .HasConversion<string>();
            modelBuilder.Entity<Test>()
               .Property(a => a.Category)
               .HasConversion<string>();
            modelBuilder.Entity<Test>()
               .Property(a => a.TestType)
               .HasConversion<string>();
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AssessmentCriteria> AssessmentCriteria { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecord { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollment { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Lesson> Lesson { get; set; }
        public DbSet<MCQOption> MCQOption { get; set; }
        public DbSet<OTP> OTP { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<ScheduleWork> ScheduleWork { get; set; }
        public DbSet<StudentTest> StudentTest { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<SyllabusSchedule> SyllabusSchedule { get; set; }
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<Test> Test { get; set; }
        public DbSet<TestEvent> TestEvent { get; set; }
        public DbSet<TestSection> TestSection { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<SyllabusScheduleTest> SyllabusScheduleTests { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }

    }
}
