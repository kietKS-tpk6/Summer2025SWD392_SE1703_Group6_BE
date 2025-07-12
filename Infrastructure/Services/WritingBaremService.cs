using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
namespace Infrastructure.Services
{
    public class WritingBaremService : IWritingBaremService
    {
        private readonly IWritingBaremRepository _writingBaremRepository;
        private readonly IQuestionRepository _questionRepository;

        public WritingBaremService(IWritingBaremRepository writingBaremRepository, IQuestionRepository questionRepository)
        {
            _writingBaremRepository = writingBaremRepository;
            _questionRepository = questionRepository;
        }

        public async Task<OperationResult<bool>> ValidateCreateBaremsAsync(List<CreateWritingBaremDTO> barems)
        {
            // Kiểm tra dữ liệu từng bản ghi
            foreach (var b in barems)
            {
                if (string.IsNullOrWhiteSpace(b.CriteriaName))
                    return OperationResult<bool>.Fail("Tên tiêu chí không được để trống.");

                if (b.MaxScore <= 0)
                    return OperationResult<bool>.Fail("Điểm tối đa phải lớn hơn 0.");

                if (string.IsNullOrWhiteSpace(b.QuestionID))
                    return OperationResult<bool>.Fail("Mỗi barem phải có QuestionID.");
            }

            // Kiểm tra tồn tại của QuestionID
            var questionIDs = barems.Select(b => b.QuestionID).Distinct().ToList();
            var validQuestions = await _questionRepository.GetByIDsAsync(questionIDs);
            var validIDs = validQuestions.Select(q => q.QuestionID).ToHashSet();

            var invalidIDs = questionIDs.Where(q => !validIDs.Contains(q)).ToList();

            if (invalidIDs.Any())
                return OperationResult<bool>.Fail($"Không tìm thấy QuestionID: {string.Join(", ", invalidIDs)}");

            return OperationResult<bool>.Ok(true);
        }
        public async Task<OperationResult<bool>> ValidateWritingBaremsAsync(List<CreateWritingBaremDTO> barems)
        {
            var questionIDs = barems.Select(b => b.QuestionID).Distinct().ToList();

            // Lấy các câu hỏi kèm section
            var questions = await _questionRepository.GetQuestionsWithSectionByIdsAsync(questionIDs);
            if (questions == null || !questions.Any())
                return OperationResult<bool>.Fail("Không tìm thấy các câu hỏi.");

            foreach (var q in questions)
            {
                if (q.TestSection == null || q.TestSection.TestSectionType != TestFormatType.Writing)
                    return OperationResult<bool>.Fail($"Câu hỏi {q.QuestionID} không thuộc section kiểu Writing.");

                var sectionScore = q.Score; 

                var dbBarems = await _writingBaremRepository.GetByQuestionIDAsync(q.QuestionID);
                var totalCurrentScore = dbBarems.Sum(b => b.MaxScore);

                var newBarems = barems.Where(b => b.QuestionID == q.QuestionID).ToList();
                var totalNewScore = newBarems.Sum(b => b.MaxScore);

                var combinedScore = totalCurrentScore + totalNewScore;

                if (combinedScore > sectionScore)
                {
                    return OperationResult<bool>.Fail(
                        $"Tổng điểm các tiêu chí cho câu hỏi {q.QuestionID} vượt quá điểm tối đa của question ({sectionScore})."
                    );
                }
            }

            return OperationResult<bool>.Ok(true);
        }


        public async Task<OperationResult<bool>> CreateWritingBaremsAsync(List<CreateWritingBaremDTO> barems)
        {
            var questionIDs = barems.Select(b => b.QuestionID).Distinct().ToList();

            // Lấy thông tin Question + Section
            var questions = await _questionRepository.GetQuestionsWithSectionByIdsAsync(questionIDs);
            if (questions == null || !questions.Any())
                return OperationResult<bool>.Fail("Không tìm thấy các câu hỏi tương ứng.");

            // Lọc ra những câu hỏi thuộc section Writing
            var validQuestionIDs = questions
                .Where(q => q.TestSection != null && q.TestSection.TestSectionType == TestFormatType.Writing)
                .Select(q => q.QuestionID)
                .ToHashSet();

            // Nếu không có câu nào hợp lệ
            if (!validQuestionIDs.Any())
                return OperationResult<bool>.Fail("Không có câu hỏi nào thuộc phần Writing để tạo barem.");

            var validBarems = barems
                .Where(b => validQuestionIDs.Contains(b.QuestionID))
                .Select(b => new WritingBarem
                {
                    WritingBaremID = Guid.NewGuid().ToString("N")[..6],
                    QuestionID = b.QuestionID,
                    CriteriaName = b.CriteriaName,
                    MaxScore = b.MaxScore,
                    Description = b.Description
                })
                .ToList();

            if (!validBarems.Any())
                return OperationResult<bool>.Fail("Không có barem nào hợp lệ để thêm.");

            await _writingBaremRepository.AddRangeAsync(validBarems);
            return OperationResult<bool>.Ok(true, "Tạo barem thành công.");
        }

        public async Task<OperationResult<List<WritingBaremDTO>>> GetByQuestionIDAsync(string questionID)
        {
            var barems = await _writingBaremRepository.GetByQuestionIDAsync(questionID);

            var result = barems.Select(b => new WritingBaremDTO
            {
                WritingBaremID = b.WritingBaremID,
                CriteriaName = b.CriteriaName,
                MaxScore = b.MaxScore,
                Description = b.Description,
            }).ToList();

            return OperationResult<List<WritingBaremDTO>>.Ok(result);
        }
        public async Task<OperationResult<bool>> ValidateUpdateBaremAsync(UpdateWritingBaremCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.CriteriaName))
                return OperationResult<bool>.Fail("Tên tiêu chí không được để trống.");

            if (command.MaxScore <= 0)
                return OperationResult<bool>.Fail("Điểm tối đa phải lớn hơn 0.");

            var existing = await _writingBaremRepository.GetByIDAsync(command.WritingBaremID);
            if (existing == null)
                return OperationResult<bool>.Fail("Không tìm thấy barem chấm điểm.");

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> UpdateWritingBaremAsync(UpdateWritingBaremCommand command)
        {
            // Lấy Barem cần cập nhật
            var barem = await _writingBaremRepository.GetByIDAsync(command.WritingBaremID);
            if (barem == null)
                return OperationResult<bool>.Fail("Không tìm thấy barem chấm điểm.");

            // Lấy thông tin câu hỏi
            var question = await _questionRepository.GetByIdAsync(barem.QuestionID);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi liên quan.");

            // Lấy tất cả các barem hiện có của câu hỏi (trừ cái đang cập nhật)
            var existingBarems = await _writingBaremRepository.GetByQuestionIDAsync(barem.QuestionID);
            var totalOtherScore = existingBarems
                .Where(b => b.WritingBaremID != barem.WritingBaremID)
                .Sum(b => b.MaxScore);

            // Tổng mới sau khi update
            var combinedScore = totalOtherScore + command.MaxScore;

            if (combinedScore > question.Score)
            {
                return OperationResult<bool>.Fail(
                    $"Tổng điểm các tiêu chí vượt quá điểm tối đa của câu hỏi ({question.Score})."
                );
            }

            // Cập nhật
            barem.CriteriaName = command.CriteriaName;
            barem.MaxScore = command.MaxScore;
            barem.Description = command.Description;

            await _writingBaremRepository.UpdateAsync(barem);

            return OperationResult<bool>.Ok(true, "Cập nhật thành công.");
        }


        public async Task<OperationResult<bool>> SoftDeleteWritingBaremAsync(string writingBaremID)
        {
            var barem = await _writingBaremRepository.GetByIDAsync(writingBaremID);
            if (barem == null)
                return OperationResult<bool>.Fail("Không tìm thấy barem để xoá.");

            barem.IsActive = false;

            await _writingBaremRepository.UpdateAsync(barem);
            return OperationResult<bool>.Ok(true, "Xóa mềm thành công.");
        }

    }

}
