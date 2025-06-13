using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateTestSchedulesToSlotsCommandHandler : IRequestHandler<UpdateTestSchedulesToSlotsCommand, bool>
    {
        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        private readonly ISyllabusScheduleService _syllabusScheduleService;

        public UpdateTestSchedulesToSlotsCommandHandler(ISyllabusScheduleTestService syllabusScheduleTestService, IAssessmentCriteriaService assessmentCriteriaService, ISyllabusScheduleService syllabusScheduleService)
        {
            _syllabusScheduleTestService = syllabusScheduleTestService;
            _assessmentCriteriaService = assessmentCriteriaService;
            _syllabusScheduleService = syllabusScheduleService;
        }

        public async Task<bool> Handle(UpdateTestSchedulesToSlotsCommand request, CancellationToken cancellationToken)
        {
            // Bước 1: Chuẩn hóa enums
            TestCategory? parsedCategory;
            TestType? parsedType;

            try
            {
                parsedCategory = _syllabusScheduleTestService.NormalizeTestCategory(request.TestCategory);
                parsedType = _syllabusScheduleTestService.NormalizeTestType(request.TestType);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Loại kiểm tra không hợp lệ: {ex.Message}");
            }

            if (parsedCategory == null || parsedType == null)
                throw new ArgumentException("Không thể xác định loại kiểm tra hoặc danh mục kiểm tra.");

            // Bước 2: Kiểm tra slot có cho phép kiểm tra không
            var slotAllowed = await _syllabusScheduleService.slotAllowToTestAsync(request.SyllabusScheduleID);
            if (!slotAllowed)
                throw new InvalidOperationException($"Slot với ID {request.SyllabusScheduleID} không được phép thực hiện kiểm tra.");

            // Bước 3: Kiểm tra bài test có trong AssessmentCriteria không
            var isDefined = await _assessmentCriteriaService.IsTestDefinedInCriteriaAsync(
                request.syllabusId,
                parsedCategory.Value,
                parsedType.Value);

            if (!isDefined)
                throw new InvalidOperationException($"Bài kiểm tra {parsedCategory.Value} - {parsedType.Value} chưa được định nghĩa trong tiêu chí đánh giá của syllabus {request.syllabusId}.");

            // Bước 4: Kiểm tra bài test có vượt quá số lượng cho phép không
            var isOverLimit = await _syllabusScheduleTestService.IsTestOverLimitAsync(
                request.syllabusId,
                parsedCategory.Value,
                parsedType.Value,
                request.SyllabusScheduleTestsId);

            if (isOverLimit)
                throw new InvalidOperationException($"Số lượng bài kiểm tra {parsedCategory.Value} - {parsedType.Value} đã vượt quá giới hạn cho phép trong syllabus {request.syllabusId}.");

            // Bước 5: Kiểm tra thứ tự test
            var isOrderValid = await _syllabusScheduleService.ValidateTestPositionAsync(
                request.syllabusId,
                request.SyllabusScheduleID,
                request.TestCategory);

            if (!isOrderValid)
                throw new InvalidOperationException($"Thứ tự bài kiểm tra {request.TestCategory} không hợp lệ trong syllabus {request.syllabusId} tại slot {request.SyllabusScheduleID}.");

            // Bước 7: Thực hiện cập nhật test
            return await _syllabusScheduleTestService.UpdateTestToSyllabusAsync(request);
        }

    }
}
