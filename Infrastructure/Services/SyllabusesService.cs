using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
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
        private readonly ISubjectRepository _iSubjectRepository;


        public SyllabusesService(ISyllabusesRepository syllabusesRepository, ISubjectRepository subjectRepository)
        {
            _iSyllabusesRepository = syllabusesRepository;
            _iSubjectRepository = subjectRepository;
        }

        public async Task<bool> IsValidSyllabusStatusForSubjectAsync(string subjectID)
        {

            return await _iSyllabusesRepository.IsValidSyllabusStatusForSubjectAsync(subjectID);
        }
            
        public async Task<bool> createSyllabuses(CreateSyllabusesCommand createSyllabusesCommand)
        {
            if (createSyllabusesCommand == null)
                throw new ArgumentNullException(nameof(createSyllabusesCommand));

            var numberOfSyllabuses  = (await _iSyllabusesRepository.GetNumbeOfSyllabusAsync()) + 2;

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
                return true;
            return false;
        }

        public async Task<bool> ExistsSyllabusAsync(string SyllabusID)
        {
            return await _iSyllabusesRepository.ExistsSyllabusAsync(SyllabusID);
        }


        public async Task<string> UpdateSyllabusesAsync(UpdateSyllabusesCommand updateSyllabusesCommand)
        {
            var normalizedStatus = NormalizeStatus(updateSyllabusesCommand.Status);
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var syllabus = new Syllabus
            {
                SyllabusID = updateSyllabusesCommand.SyllabusID,
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

            var trimmedStatus = status.Trim();

            // Thử parse trực tiếp với IgnoreCase để chấp nhận các biến thể như DrAftEd, DRAFTED, drafted
            if (Enum.TryParse<Domain.Enums.SyllabusStatus>(trimmedStatus, ignoreCase: true, out var result))
            {
                return result;
            }

            // Nếu không parse được thì báo lỗi
            throw new ArgumentException($"Status '{status}' không hợp lệ");
        }

        public Task<SyllabusDTO> getSyllabusBySubjectID(string SyllabusID)
        {
            return _iSyllabusesRepository.getSyllabusBySubjectID(SyllabusID);
        }

        public Task<bool> DeleteSyllabusById(string SyllabusID)
        {
            return _iSyllabusesRepository.deleteSyllabusById(SyllabusID);
        }
    }
}
