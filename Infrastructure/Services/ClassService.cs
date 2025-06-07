using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
namespace Infrastructure.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        public ClassService(IClassRepository classRepository, ISubjectRepository subjectRepository, IEnrollmentRepository enrollmentRepository)
        {
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> CreateClassAsync(ClassCreateCommand classCreateCommand)
        {
            var numberOfClasses = await _classRepository.CountAsync();
            string newClassID = "CL" + numberOfClasses.ToString("D4"); 

            var newClass = new Class
            {
                ClassID = newClassID,
                LecturerID = classCreateCommand.LecturerID,
                SubjectID = classCreateCommand.SubjectID,
                ClassName = classCreateCommand.ClassName,
                MinStudentAcp = classCreateCommand.MinStudentAcp,
                MaxStudentAcp = classCreateCommand.MaxStudentAcp,
                PriceOfClass = classCreateCommand.PriceOfClass,
                Status = ClassStatus.Pending,
                CreateAt = DateTime.UtcNow,
                TeachingStartTime = classCreateCommand.TeachingStartTime,
                ImageURL = classCreateCommand.ImageURL,
            };

            return await _classRepository.CreateAsync(newClass);
        }
        public async Task<List<ClassDetailForPaymentDTO>> GetAllClassesForPaymentAsync()
        {
            var classes = await _classRepository.GetAllClassesAsync(includeInactive: false);
            var result = new List<ClassDetailForPaymentDTO>();

            foreach (var classEntity in classes)
            {
                var subject = await _subjectRepository.GetSubjectByIdAsync(classEntity.SubjectID);
                var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classEntity.ClassID);

                result.Add(new ClassDetailForPaymentDTO
                {
                    ClassID = classEntity.ClassID,
                    ClassName = classEntity.ClassName,
                    SubjectName = subject?.SubjectName ?? "Unknown",
                    PriceOfClass = classEntity.PriceOfClass,
                    TeachingStartTime = classEntity.TeachingStartTime,
                    ImageURL = classEntity.ImageURL,
                    MaxStudentAcp = classEntity.MaxStudentAcp,
                    CurrentEnrollments = currentEnrollments,
                    CanEnroll = classEntity.Status == ClassStatus.Open && currentEnrollments < classEntity.MaxStudentAcp
                });
            }

            return result;
        }

        public async Task<ClassDetailForPaymentDTO> GetClassDetailForPaymentAsync(string classId)
        {
            var classEntity = await _classRepository.GetClassByIdAsync(classId);
            if (classEntity == null) return null;

            var subject = await _subjectRepository.GetSubjectByIdAsync(classEntity.SubjectID);
            var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classEntity.ClassID);

            return new ClassDetailForPaymentDTO
            {
                ClassID = classEntity.ClassID,
                ClassName = classEntity.ClassName,
                SubjectName = subject?.SubjectName ?? "Unknown",
                PriceOfClass = classEntity.PriceOfClass,
                TeachingStartTime = classEntity.TeachingStartTime,
                ImageURL = classEntity.ImageURL,
                MaxStudentAcp = classEntity.MaxStudentAcp,
                CurrentEnrollments = currentEnrollments,
                CanEnroll = classEntity.Status == ClassStatus.Open && currentEnrollments < classEntity.MaxStudentAcp
            };
        }

        public async Task<bool> IsClassAvailableForEnrollmentAsync(string classId)
        {
            var classEntity = await _classRepository.GetClassByIdAsync(classId);
            if (classEntity == null) return false;

            var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classId);
            return classEntity.Status == ClassStatus.Open && currentEnrollments < classEntity.MaxStudentAcp;
        }

        public async Task<int> GetAvailableSlotsAsync(string classId)
        {
            var classEntity = await _classRepository.GetClassByIdAsync(classId);
            if (classEntity == null) return 0;

            var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classId);
            return Math.Max(0, classEntity.MaxStudentAcp - currentEnrollments);
        }
    }
}
