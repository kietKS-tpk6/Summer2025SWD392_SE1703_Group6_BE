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
        public SyllabusesService(SyllabusesRepository syllabusesRepository)
        {
            _iSyllabusesRepository = syllabusesRepository;
        }

        public async Task<string> createSyllabuses(CreateSyllabusesCommand createSyllabusesCommand)
        {
            if (createSyllabusesCommand == null)
                throw new ArgumentNullException(nameof(createSyllabusesCommand));

            if (string.IsNullOrWhiteSpace(createSyllabusesCommand.SubjectID))
                throw new ArgumentException("SubjectID is required", nameof(createSyllabusesCommand.SubjectID));

            if (string.IsNullOrWhiteSpace(createSyllabusesCommand.AccountID))
                throw new ArgumentException("AccountID is required", nameof(createSyllabusesCommand.AccountID));

            var numberOfSyllabuses = (await _iSyllabusesRepository.GetNumbeOfSyllabusAsync());

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);


            var syl = new Syllabus();
            syl.SyllabusID = "SYL" + numberOfSyllabuses;
            syl.SubjectID = createSyllabusesCommand.SubjectID;
            syl.CreateBy = createSyllabusesCommand.AccountID;
            syl.CreateAt = vietnamTime;
            syl.UpdateBy = null;
            syl.UpdateAt = null;
            syl.Description = createSyllabusesCommand.Description;
            syl.Note = createSyllabusesCommand.Note;

            if(await _iSyllabusesRepository.CreateSyllabusesAsync(syl))
                return "Tạo chương trình học thành công";
            return "Có lỗi xảy ra khi tạo chương trình học";
        }
  
    }
}
