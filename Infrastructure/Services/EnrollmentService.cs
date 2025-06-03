using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClassRepository _classRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IPaymentRepository paymentRepository,
            IClassRepository classRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _paymentRepository = paymentRepository;
            _classRepository = classRepository;
        }

        public async Task<string> GenerateNextEnrollmentIdAsync()
        {
            var numberOfEnrollments = await _enrollmentRepository.GetTotalEnrollmentsCountAsync();
            return $"CE{(numberOfEnrollments + 1):D4}";
        }

        public async Task<string> CreateEnrollmentAsync(CreateEnrollmentCommand command)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(command.PaymentID);
            if (payment == null)
            {
                return "Payment not found";
            }

            if (payment.Status != PaymentStatus.Paid)
            {
                return "Payment is not completed";
            }

            var isAlreadyEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(command.StudentID, command.ClassID);
            if (isAlreadyEnrolled)
            {
                return "Student is already enrolled in this class";
            }

            var classEntity = await _classRepository.GetClassByIdAsync(command.ClassID);
            if (classEntity == null)
            {
                return "Class not found";
            }

            var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(command.ClassID);
            if (currentEnrollments >= classEntity.MaxStudentAcp)
            {
                return "Class is full";
            }

            var enrollmentId = await GenerateNextEnrollmentIdAsync();

    
            var enrollment = new ClassEnrollment
            {
                ClassEnrollmentID = enrollmentId,
                StudentID = command.StudentID,
                ClassID = command.ClassID,
                EnrolledDate = DateTime.Now,
                Status = EnrollmentStatus.Actived
            };

            var result = await _enrollmentRepository.CreateEnrollmentAsync(enrollment);

            if (result.Contains("successfully"))
            {
                return $"Enrollment created successfully with ID: {enrollmentId}";
            }

            return result;
        }

        public async Task<List<ClassDetailForPaymentDTO>> GetMyClassesAsync(string studentId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            var result = new List<ClassDetailForPaymentDTO>();

            foreach (var enrollment in enrollments)
            {
                var classEntity = await _classRepository.GetClassByIdAsync(enrollment.ClassID);
                if (classEntity != null)
                {
                    var payment = await _paymentRepository.GetPaymentByIdAsync(
                        (await _paymentRepository.GetPaymentsByAccountIdAsync(studentId))
                        .FirstOrDefault(p => p.ClassID == enrollment.ClassID)?.PaymentID ?? "");

                    result.Add(new ClassDetailForPaymentDTO
                    {
                        ClassID = classEntity.ClassID,
                        ClassName = classEntity.ClassName,
                        SubjectName = classEntity.Subject?.SubjectName ?? "Unknown",
                        PriceOfClass = classEntity.PriceOfClass,
                        TeachingStartTime = classEntity.TeachingStartTime,
                        ImageURL = classEntity.ImageURL,
                        LecturerName = classEntity.Lecturer?.FullName ?? "Unknown",
                        MaxStudentAcp = classEntity.MaxStudentAcp,
                        CurrentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classEntity.ClassID),
                        CanEnroll = false 
                    });
                }
            }

            return result;
        }

        public async Task<bool> IsStudentEnrolledAsync(string studentId, string classId)
        {
            return await _enrollmentRepository.IsStudentEnrolledAsync(studentId, classId);
        }

        public async Task<int> GetClassCurrentEnrollmentsAsync(string classId)
        {
            return await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(classId);
        }
    }
}