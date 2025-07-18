using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
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
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IStudentMarkRepository _studentMarkRepository;
        private readonly ISubjectRepository _subjectRepository;


        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IPaymentRepository paymentRepository,
            IClassRepository classRepository,
            IAssessmentCriteriaRepository assessmentCriteria,IStudentMarkRepository studentMarkRepository,ISubjectRepository subjectRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _paymentRepository = paymentRepository;
            _classRepository = classRepository;
            _assessmentCriteriaRepository = assessmentCriteria;
            _studentMarkRepository = studentMarkRepository;
            _subjectRepository = subjectRepository;
        }

        public async Task<string> GenerateNextEnrollmentIdAsync()
        {
            var numberOfEnrollments = await _enrollmentRepository.GetTotalEnrollmentsCountAsync();
            return $"CE{(numberOfEnrollments + 1):D4}";
        }

        public async Task<bool> HasScheduleConflictAsync(string studentId, string newClassId)
        {
            try
            {
                var studentLessons = await _enrollmentRepository.GetLessonsByStudentIdAsync(studentId);

                var newClassLessons = await _enrollmentRepository.GetLessonsByClassIdAsync(newClassId);

                foreach (var newLesson in newClassLessons)
                {
                    var newLessonStart = newLesson.StartTime;
                    var newLessonEnd = newLessonStart.AddMinutes(newLesson.SyllabusSchedule?.DurationMinutes ?? 60);

                    foreach (var existingLesson in studentLessons)
                    {
                        var existingLessonStart = existingLesson.StartTime;
                        var existingLessonEnd = existingLessonStart.AddMinutes(existingLesson.SyllabusSchedule?.DurationMinutes ?? 60);

                        if (IsTimeOverlap(newLessonStart, newLessonEnd, existingLessonStart, existingLessonEnd))
                        {
                            return true; 
                        }
                    }
                }

                return false; 
            }
            catch (Exception)
            {
                return true; 
            }
        }

        private bool IsTimeOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && start2 < end1;
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

            var classEntity = await _classRepository.GetByIdAsync(command.ClassID);
            if (classEntity == null)
            {
                return "Class not found";
            }

            var currentEnrollments = await _enrollmentRepository.GetClassCurrentEnrollmentsAsync(command.ClassID);
            if (currentEnrollments >= classEntity.Data.MaxStudentAcp)
            {
                return "Class is full";
            }

            var hasConflict = await HasScheduleConflictAsync(command.StudentID, command.ClassID);
            if (hasConflict)
            {
                return "Schedule conflict: This class conflicts with your existing class schedule";
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
            //tạo 
            //var listClass = await _classRepository.GetListClassByIdAsync(enrollments.FirstOrDefault().ClassID);

            foreach (var enrollment in enrollments)
            {
                var classResult = await _classRepository.GetByIdAsync(enrollment.ClassID);
                var classEntity = classResult.Data;
                //foreach ( var classRes in listClass)
if (classEntity != null)
                {
                    //var payment = await _paymentRepository.GetPaymentByIdAsync(
                    //    (await _paymentRepository.GetPaymentsByAccountIdAsync(studentId))
                    //    .FirstOrDefault(p => p.ClassID == enrollment.ClassID)?.PaymentID ?? "");

                    result.Add(new ClassDetailForPaymentDTO
                    {
                        ClassID = classEntity.ClassID,
                        ClassName = classEntity.ClassName,
                      //ClassStatus = classEntity.Status,
                        ClassStatus = classEntity.Status.ToString(),
                        SubjectName = classEntity.Subject?.SubjectName ?? "Unknown",
                        ImageURL = classEntity.ImageURL,
                        LecturerName = classEntity.Lecturer.Fullname
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
        //kit
        public async Task<OperationResult<List<Lesson>>> GetLessonsByClassIDAsync(string classID)
        {
            var lessons = await _classRepository.GetByClassIDAsync(classID);
            return OperationResult<List<Lesson>>.Ok(lessons);
        }
        public async Task<OperationResult<int>> CountActiveStudentsByLecturerAsync(string lecturerId)
        {
            var count = await _enrollmentRepository.CountActiveStudentsOfLecturerAsync(lecturerId);
            return OperationResult<int>.Ok(count, OperationMessages.RetrieveSuccess("số sinh viên đang học"));
        }
        public async Task<OperationResult<bool>> UpdateStatusesByClassIDAsync(string classID)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(classID))
                {
                    return OperationResult<bool>.Fail("Class ID cannot be null or empty");
                }

                // 1. Lấy thông tin lớp học và kiểm tra trạng thái
                var classInfo = await _classRepository.GetByIdAsync(classID);
                if (classInfo == null || classInfo.Data == null)
                {
                    return OperationResult<bool>.Fail("Class not found");
                }

                // Chỉ cập nhật status khi lớp học đã kết thúc hoặc đang trong trạng thái có thể đánh giá
                if (classInfo.Data.Status != ClassStatus.Completed && classInfo.Data.Status != ClassStatus.Ongoing)
                {
                    return OperationResult<bool>.Fail("Class is not in a valid state for status update");
                }

                // 2. Lấy danh sách tất cả enrollments trong class
                var enrollments = await _enrollmentRepository.GetEnrollmentsByClassIdAsync(classID);
                if (!enrollments.Any())
                {
                    return OperationResult<bool>.Ok(true); // Không có enrollment nào để cập nhật
                }

                // 3. Lấy subject info để lấy MinAverageScoreToPass
                var subject = await _subjectRepository.GetSubjectByIdAsync(classInfo.Data.SubjectID);
                if (subject == null)
                {
                    return OperationResult<bool>.Fail("Subject not found");
                }

                // 4. Lấy assessment criteria của subject
                var criteriasResult = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(subject.SubjectID);
                if (criteriasResult.Data == null || !criteriasResult.Data.Any())
                {
                    return OperationResult<bool>.Fail("No assessment criteria found for this subject");
                }

                // 5. Lấy thông tin cấu hình điểm tối thiểu để pass
                decimal minAverageToPass = (decimal)subject.MinAverageScoreToPass;

                // 6. Lấy tất cả điểm của students trong class
                var studentIds = enrollments.Select(e => e.StudentID).ToList();
                var updatedCount = 0;

                // 7. Duyệt qua từng enrollment và cập nhật status
                foreach (var enrollment in enrollments)
                {
                    // Bỏ qua những enrollment đã bị cancel
                    if (enrollment.Status == EnrollmentStatus.Cancelled)
                        continue;

                    // Lấy điểm của student hiện tại
                    var studentMarks = await _studentMarkRepository.GetMarksByStudentAndClassAsync(enrollment.StudentID, classID);

                    // Xác định status mới
                    EnrollmentStatus newStatus;

                    // Nếu đã bị cancel thì giữ nguyên
                    if (enrollment.Status == EnrollmentStatus.Cancelled)
                    {
                        newStatus = EnrollmentStatus.Cancelled;
                    }
                    // Nếu class chưa kết thúc thì để Active
                    else if (classInfo.Data.Status == ClassStatus.Ongoing)
                    {
                        newStatus = EnrollmentStatus.Actived;
                    }
                    // Nếu class đã kết thúc, kiểm tra điểm để xác định Pass/Fail
                    else if (classInfo.Data.Status == ClassStatus.Completed)
                    {
                        // Kiểm tra xem student có điểm không
                        if (!studentMarks.Any())
                        {
                            // Nếu không có điểm nào thì coi như failed
                            newStatus = EnrollmentStatus.Failed;
                        }
                        else
                        {
                            // Sử dụng hàm IsStudentQualified để kiểm tra
                            bool isQualified = IsStudentQualified(enrollment.StudentID, studentMarks, criteriasResult.Data, minAverageToPass);
                            newStatus = isQualified ? EnrollmentStatus.Passed : EnrollmentStatus.Failed;
                        }
                    }
                    else
                    {
                        // Mặc định giữ nguyên status hiện tại
                        newStatus = enrollment.Status;
                    }

                    // Cập nhật nếu status thay đổi
                    if (enrollment.Status != newStatus)
                    {
                        enrollment.Status = newStatus;
                        await _enrollmentRepository.UpdateEnrollmentAsync(enrollment);
                        updatedCount++;
                    }
                }

                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"An error occurred: {ex.Message}");
            }

        }
        private bool IsStudentQualified(string studentID, List<StudentMark> studentMarks, List<AssessmentCriteriaDTO> criterias, decimal minAverageToPass)
        {
            decimal weightedTotal = 0;
            decimal totalWeight = 0;

            foreach (var criteria in criterias)
            {
                var criteriaMarks = studentMarks
                    .Where(m => m.AssessmentCriteriaID == criteria.AssessmentCriteriaID)
                    .Select(m => m.Mark)
                    .Where(m => m.HasValue)
                    .Select(m => m.Value)
                    .ToList();

                // Nếu không có điểm nào thì bỏ qua tiêu chí
                if (!criteriaMarks.Any())
                    continue;

                decimal avg = criteriaMarks.Average();
                decimal weight = (decimal)(criteria.WeightPercent ?? 0);

                weightedTotal += avg * weight;
                totalWeight += weight;
            }

            if (totalWeight == 0)
                return false;

            decimal finalScore = weightedTotal / totalWeight;
            return finalScore >= minAverageToPass;
        }

    }
}