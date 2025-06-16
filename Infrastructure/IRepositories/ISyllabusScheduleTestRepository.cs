using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IRepositories
{
    public interface ISyllabusScheduleTestRepository
    {
        Task<List<SyllabusScheduleTestDTO>> GetTestsBySyllabusIdAsync(string syllabusId);
        Task<bool> AddAsync(SyllabusScheduleTest entity);
        Task<bool> UpdateAsync(SyllabusScheduleTest entity);

        Task<bool> HasTestAsync(string syllabusScheduleId);
        Task<bool> RemoveTestFromSlotAsyncs(string syllabusScheduleId);
        Task<SyllabusScheduleTest> GetByIdAsync(int SyllabusScheduleTestID);
        Task<List<SyllabusScheduleTestDTO>> GetExamAddedToSyllabusAsync(List<string> slotAllowToTest);

        //Hàm cụa Kho - 
        Task<SyllabusScheduleTest?> GetSyllabusScheduleTestBySyllabusScheduleIdAsync(string syllabusScheduleID);

    }
}
