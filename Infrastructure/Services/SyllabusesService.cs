using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Infrastructure.Services
{
    public class SyllabusesService : ISyllabusesService
    {
        private readonly ISyllabusesRepository _iSyllabusesRepository;

        public SyllabusesService(ISyllabusesRepository syllabusesRepository)
        {
            _iSyllabusesRepository = syllabusesRepository;
        }

        public async Task<string> createSyllabuses(CreateSyllabusesCommand createSyllabusesCommand)
        {
            if (createSyllabusesCommand == null)
                throw new ArgumentNullException(nameof(createSyllabusesCommand));

            var numberOfSyllabuses = (await _iSyllabusesRepository.GetNumbeOfSyllabusAsync());

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var syl = new Syllabus();
            syl.SyllabusID = "SY" + numberOfSyllabuses.ToString("D4");
            syl.SubjectID = createSyllabusesCommand.SubjectID;
            syl.CreateBy = createSyllabusesCommand.AccountID;
            syl.CreateAt = vietnamTime;
            syl.UpdateBy = null;
            syl.UpdateAt = null;
            syl.Description = createSyllabusesCommand.Description;
            syl.Note = createSyllabusesCommand.Note;
            syl.Status = Domain.Enums.SyllabusStatus.Drafted;


            if (await _iSyllabusesRepository.CreateSyllabusesAsync(syl))
                return "Tạo chương trình học thành công";
            return "Có lỗi xảy ra khi tạo chương trình học";
        }

        public async Task<string> UpdateSyllabusesAsync(UpdateSyllabusesCommand updateSyllabusesCommand)
        {
            if (updateSyllabusesCommand == null)
                throw new ArgumentNullException(nameof(updateSyllabusesCommand));
            var checkSyl = await _iSyllabusesRepository.ExistsSyllabusAsync(updateSyllabusesCommand.SyllabusID);
            if (!checkSyl) return "Chương trình học không tồn tại.";
            var normalizedStatus = NormalizeStatus(updateSyllabusesCommand.Status);
            var numberOfSyllabuses = await _iSyllabusesRepository.GetNumbeOfSyllabusAsync();
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var syllabus = new Syllabus
            {
                SyllabusID = updateSyllabusesCommand.SyllabusID,
                SubjectID = updateSyllabusesCommand.SubjectID,
                UpdateBy = updateSyllabusesCommand.AccountID,
                UpdateAt = vietnamTime,
                Description = updateSyllabusesCommand.Description,
                Note = updateSyllabusesCommand.Note,
                Status = normalizedStatus
            };

            var result = await _iSyllabusesRepository.UpdateSyllabusesAsync(syllabus);
            return result ? "Cập nhật chương trình học thành công" : "Có lỗi xảy ra khi cập nhật chương trình học";
        }

        /// <summary>
        /// Chuẩn hóa Status: viết hoa chữ cái đầu
        /// Nếu không có status hoặc không hợp lệ thì mặc định là Drafted
        /// </summary>
        /// <param name="status">Status cần chuẩn hóa</param>
        /// <returns>Status đã được chuẩn hóa</returns>
        private Domain.Enums.SyllabusStatus NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return Domain.Enums.SyllabusStatus.Drafted;

            // Viết hoa chữ cái đầu và trim
            var normalizedStatus = status.Trim();
            if (normalizedStatus.Length > 0)
            {
                normalizedStatus = char.ToUpper(normalizedStatus[0]) + normalizedStatus.Substring(1).ToLower();
            }

            // Parse thành enum
            if (Enum.TryParse<Domain.Enums.SyllabusStatus>(normalizedStatus, out var result))
            {
                return result;
            }

            // Nếu không parse được thì trả về Drafted
            return Domain.Enums.SyllabusStatus.Drafted;
        }

       

    }
}
