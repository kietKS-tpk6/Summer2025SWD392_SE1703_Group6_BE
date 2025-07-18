using Application.Common.Constants;
using Application.IServices;
using Domain.Entities;
using Infrastructure.IRepositories;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing;
using Application.DTOs;
using System;
using Domain.Enums;

public class CertificateService : ICertificateService
{
    private readonly IEmailService _emailService;
    private readonly ISystemConfigService _systemConfigService;
    private readonly IAccountRepository _accountRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IClassRepository _classRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentMarkRepository _studentMarkRepository;
    private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;

    public CertificateService(
        IEmailService emailService,
        ISystemConfigService systemConfigService,
        IAccountRepository accountRepository,
        ISubjectRepository subjectRepository,
        IClassRepository classRepository,
        IEnrollmentRepository enrollmentRepository,
        IStudentMarkRepository studentMarkRepository,
        IAssessmentCriteriaRepository assessmentCriteriaRepository)
    {
        _emailService = emailService;
        _systemConfigService = systemConfigService;
        _accountRepository = accountRepository;
        _subjectRepository = subjectRepository;
        _classRepository = classRepository;
        _enrollmentRepository = enrollmentRepository;
        _studentMarkRepository = studentMarkRepository;
        _assessmentCriteriaRepository = assessmentCriteriaRepository;
    }

    public async Task<OperationResult<bool>> SendCertificateToStudentByStudentIDAndSubjectIDAsync(string studentID, string classID)
    {
        var isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(studentID, classID);
        if (!isEnrolled) return OperationResult<bool>.Fail("Học sinh không nằm trong lớp.");

        var marks = await _studentMarkRepository.GetMarksByStudentAndClassAsync(studentID, classID);
        if (!marks.Any()) return OperationResult<bool>.Fail("Học sinh chưa có điểm.");

        var classInfo = await _classRepository.GetByIdAsync(classID);
        if (classInfo == null || classInfo.Data == null)
            return OperationResult<bool>.Fail("Không tìm thấy lớp.");

        if (classInfo.Data.Status != ClassStatus.Completed)
            return OperationResult<bool>.Fail("Chỉ những lớp đã hoàn thành (Completed) mới được phép gửi chứng chỉ.");

        var subject = await _subjectRepository.GetSubjectByIdAsync(classInfo.Data.SubjectID);
        var criterias = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(subject.SubjectID);

        var isQualified = IsStudentQualified(studentID, marks, criterias.Data, (decimal)subject.MinAverageScoreToPass);
        if (!isQualified) return OperationResult<bool>.Ok("Những học sinh không đủ điều kiện nhận chứng chỉ.");

        var student = await _accountRepository.GetAccountsByIdAsync(studentID);
        var lecturer = await _accountRepository.GetAccountsByIdAsync(classInfo.Data.LecturerID);

        var imageBytes = await GenerateCertificateImageAsync(student.Fullname, subject.SubjectName, lecturer.Fullname);
        if (imageBytes == null)
            return OperationResult<bool>.Fail("Không tạo được ảnh chứng chỉ.");

        var sent = await _emailService.SendCertificateWithAttachmentAsync(student.Email, student.Fullname, subject.SubjectName, imageBytes);
        if (!sent)
            return OperationResult<bool>.Fail("Gửi email thất bại.");

        return OperationResult<bool>.Ok(true, "Gửi chứng chỉ thành công.");
    }

    public async Task<OperationResult<List<CertificateSendErrorDTO>>> SendCertificatesToClassAsync(string classId)
    {
        var classInfo = await _classRepository.GetByIdAsync(classId);
        if (classInfo == null || classInfo.Data == null)
            return OperationResult<List<CertificateSendErrorDTO>>.Fail(new List<CertificateSendErrorDTO>(), "Không tìm thấy lớp.");
        if (classInfo.Data.Status != ClassStatus.Completed)
            return OperationResult<List<CertificateSendErrorDTO>>.Fail("Chỉ những lớp đã hoàn thành (Completed) mới được phép gửi chứng chỉ.");

        var enrollments = await _enrollmentRepository.GetEnrollmentsByClassIdAsync(classId);
        if (enrollments == null || !enrollments.Any())
            return OperationResult<List<CertificateSendErrorDTO>>.Fail(new List<CertificateSendErrorDTO>(), "Không có học sinh nào trong lớp.");

        var subject = await _subjectRepository.GetSubjectByIdAsync(classInfo.Data.SubjectID);
        var criterias = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(subject.SubjectID);
        var allMarks = await _studentMarkRepository.GetMarksByClassAsync(classId);
        var lecturer = await _accountRepository.GetAccountsByIdAsync(classInfo.Data.LecturerID);

        var errors = new List<CertificateSendErrorDTO>();

        foreach (var enrollment in enrollments)
        {
            var student = await _accountRepository.GetAccountsByIdAsync(enrollment.StudentID);
            var studentMarks = allMarks.Where(m => m.AccountID == enrollment.StudentID).ToList();

            if (!studentMarks.Any())
            {
                errors.Add(new CertificateSendErrorDTO
                {
                    StudentID = enrollment.StudentID,
                    FullName = student?.Fullname,
                    Reason = "Không có điểm."
                });
                continue;
            }

            var isQualified = IsStudentQualified(enrollment.StudentID, studentMarks, criterias.Data, (decimal)subject.MinAverageScoreToPass);
            if (!isQualified)
            {
                errors.Add(new CertificateSendErrorDTO
                {
                    StudentID = enrollment.StudentID,
                    FullName = student?.Fullname,
                    Reason = "Không đủ điều kiện nhận chứng chỉ."
                });
                continue;
            }

            var imageBytes = await GenerateCertificateImageAsync(student.Fullname, subject.SubjectName, lecturer.Fullname);
            if (imageBytes == null)
            {
                errors.Add(new CertificateSendErrorDTO
                {
                    StudentID = student.AccountID,
                    FullName = student.Fullname,
                    Reason = "Không tạo được ảnh chứng chỉ."
                });
                continue;
            }

            var sent = await _emailService.SendCertificateWithAttachmentAsync(student.Email, student.Fullname, subject.SubjectName, imageBytes);
            if (!sent)
            {
                errors.Add(new CertificateSendErrorDTO
                {
                    StudentID = student.AccountID,
                    FullName = student.Fullname,
                    Reason = "Gửi email thất bại."
                });
            }
        }

        if (errors.Any())
        {
            return OperationResult<List<CertificateSendErrorDTO>>.Fail(errors, "Một số học sinh không gửi chứng chỉ thành công.");
        }

        return OperationResult<List<CertificateSendErrorDTO>>.Ok(new List<CertificateSendErrorDTO>(), "Tất cả chứng chỉ đã được gửi thành công.");
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

    private async Task<string> GetFormatImgAsync()
    {
        var config = await _systemConfigService.GetConfig("certificate_template_url");
        return config.Data?.Value ?? "";
    }

    private async Task<string> GetPrincipalNameAsync()
    {
        var config = await _systemConfigService.GetConfig("principal_name");
        return config.Data?.Value ?? "";
    }

    private async Task<byte[]?> GenerateCertificateImageAsync(string studentName, string subjectName, string lectureName)
    {
        var templateUrl = await GetFormatImgAsync();
        var principalName = await GetPrincipalNameAsync();

        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(templateUrl);

        using var imageStream = new MemoryStream(imageBytes);
        using var bitmap = new Bitmap(System.Drawing.Image.FromStream(imageStream));
        using var graphics = Graphics.FromImage(bitmap);
        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

        // Load custom font
        var fontPath = Path.Combine("Fonts", "Montserrat", "Montserrat-Regular.ttf");
        var fontPathBold = Path.Combine("Fonts", "Montserrat", "Montserrat-Bold.ttf");

        if (!File.Exists(fontPath)) throw new FileNotFoundException("Font file not found at: " + Path.GetFullPath(fontPath));

        var fontCollection = new PrivateFontCollection();
        fontCollection.AddFontFile(fontPath);
        var fontCollectionBold = new PrivateFontCollection();
        fontCollectionBold.AddFontFile(fontPathBold);
        using var font = new Font(fontCollectionBold.Families[0], 36, FontStyle.Regular);
        using var studentFont = new Font(fontCollectionBold.Families[0], 60, FontStyle.Regular);
        using var brush = new SolidBrush(ColorTranslator.FromHtml("#1A6674"));

        // Student name
        var studentRect = new RectangleF((bitmap.Width - 1500) / 2, 630, 1500, 200);
        graphics.DrawString(studentName, studentFont, brush, studentRect, new StringFormat { Alignment = StringAlignment.Center });

        // Subject name
        var subjectRect = new RectangleF((bitmap.Width - 1000) / 2, 900, 1000, 70);
        graphics.DrawString(subjectName, font, brush, subjectRect, new StringFormat { Alignment = StringAlignment.Center });

        // Principal & Lecturer names
        float boxWidth = 800;
        float startX = (bitmap.Width - boxWidth * 2) / 2;
        float nameY = 1050;

        var principalRect = new RectangleF(startX, nameY, boxWidth, 70);
        var lecturerRect = new RectangleF(startX + boxWidth, nameY, boxWidth, 70);

        graphics.DrawString(principalName, font, brush, principalRect, new StringFormat { Alignment = StringAlignment.Center });
        graphics.DrawString(lectureName, font, brush, lecturerRect, new StringFormat { Alignment = StringAlignment.Center });

        using var resultStream = new MemoryStream();
        bitmap.Save(resultStream, ImageFormat.Png);
        return resultStream.ToArray();
    }
}
