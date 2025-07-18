﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace Infrastructure.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public AccountRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //public async Task<bool> AddAsync(Account account)
        //{

        //    _dbContext.Accounts.Add(account);
        //    return await _dbContext.SaveChangesAsync() > 0;
        //}
        //Bị trùng nên t comment lại - Kho
        //public async Task<string?> GetHassPassAccountWithEmailAsync(LoginCommand loginCommand)
        //{
        //    var account = await _dbContext.Accounts
        //   .FirstOrDefaultAsync(x => x.Email == loginCommand.Email);
        //    return account.HashPass;
        //}
        public async Task<Account?> GetAccountsByEmailAsync(string email)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (account  != null) return account;
            return null;
        }
        
            public async Task<Account?> GetAccountsByPhoneAsync(string phone)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (account != null) return account;
            return null;
        }
        
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<int> GetNumbeOfAccountsAsync()
        {
            return await _dbContext.Accounts.CountAsync();
        }
        public async Task<string> GetAccountNameByIDAsync(string accountID)
        {
            var account = await _dbContext.Accounts
                .Where(a => a.AccountID == accountID)
                .Select(a => new { a.FirstName, a.LastName })
                .FirstOrDefaultAsync();

            if (account == null)
                return null;

            return $"{account.LastName} {account.FirstName}".Trim();
        }

        public async Task<string?> GetHassPassAccountWithEmailAsync(LoginCommand loginCommand)
        {
            var account = await _dbContext.Accounts
           .FirstOrDefaultAsync(x => x.Email == loginCommand.Email);

            return account.HashPass;
        }
        public async Task<bool> CreateAccountAsync(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        //  Task<(List<AssessmentCriteriaDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize);

        //public async Task<PagedResult<AccountForManageDTO>> GetPaginatedAccountListAsync(int page, int pageSize, string role)
        //{
        //    var query = _dbContext.Accounts.AsQueryable();

        //    // Filter theo role nếu role không null/empty
        //    if (!string.IsNullOrEmpty(role))
        //    {
        //        if (Enum.TryParse<AccountRole>(role, true, out var roleEnum))
        //        {
        //            query = query.Where(x => x.Role == roleEnum);
        //        }
        //    }

        //    var totalItems = await query.CountAsync();
        //    var items = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(x => new AccountForManageDTO
        //        {
        //            LastName = x.LastName,
        //            FirstName = x.FirstName,
        //            Gender = x.Gender,
        //            PhoneNumber = x.PhoneNumber,
        //            Email = x.Email,
        //            BirthDate = x.BirthDate,
        //            Role = x.Role
        //        })
        //        .ToListAsync();

        //    return new PagedResult<AccountForManageDTO>
        //    {
        //        Items = items,
        //        TotalItems = totalItems,
        //        PageNumber = page,
        //        PageSize = pageSize
        //    };
        //}

        public async Task<(List<AccountForManageDTO> Items, int TotalCount)> GetPaginatedAccountListAsync(int page, int pageSize, AccountRole? role = null, Gender? gender = null, AccountStatus? status = null)
        {
            var query = _dbContext.Accounts.AsQueryable();

            // Filter theo role nếu role có giá trị
            if (role.HasValue)
            {
                query = query.Where(x => x.Role == role.Value);
            }

            // Filter theo gender nếu gender có giá trị
            if (gender.HasValue)
            {
                query = query.Where(x => x.Gender == gender.Value);
            }

            // Filter theo status nếu status có giá trị
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountForManageDTO
                {
                    AccountID = x.AccountID,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    BirthDate = x.BirthDate,
                    Role = x.Role,
                    status = x.Status  // Thêm Status vào DTO nếu cần
                })
                .ToListAsync();

            return (items, totalItems);
        }
        public async Task<OperationResult<bool>> IsLectureFreeAsync(string lecturerId, string subjectId, TimeOnly time, List<DayOfWeek> days)
        {
            try
            {
                var targetTime = time.ToTimeSpan();

                var query = from lesson in _dbContext.Lesson
                            join @class in _dbContext.Class on lesson.ClassID equals @class.ClassID
                            join schedule in _dbContext.SyllabusSchedule on lesson.SyllabusScheduleID equals schedule.SyllabusScheduleID
                            where lesson.LecturerID == lecturerId
                                  && lesson.IsActive
                                  && schedule.SubjectID == subjectId
                            select new
                            {
                                lesson.StartTime,
                                schedule.DurationMinutes
                            };

                var lessons = await query.ToListAsync();

                foreach (var lesson in lessons)
                {
                    if (days.Contains(lesson.StartTime.DayOfWeek))
                    {
                        var lessonStart = lesson.StartTime.TimeOfDay;
                        var duration = lesson.DurationMinutes ?? 0;
                        var lessonEnd = lessonStart + TimeSpan.FromMinutes(duration);

                        var proposedStart = targetTime;
                        var proposedEnd = proposedStart + TimeSpan.FromMinutes(duration);

                        if (proposedStart < lessonEnd && lessonStart < proposedEnd)
                        {
                            return OperationResult<bool>.Ok(false, "Giảng viên đang có lớp trùng thời gian.");
                        }
                    }
                }


                return OperationResult<bool>.Ok(true, "Giảng viên rảnh trong khung giờ này.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Lỗi kiểm tra lịch giảng viên: {ex.Message}");
            }
        }
        public async Task<OperationResult<List<TeachingScheduleDTO>>> GetTeachingSchedule()
        {
            try
            {
                var result = await _dbContext.Lesson
                    .Where(l => l.IsActive)
                    .Select(l => new TeachingScheduleDTO
                    {
                        LecturerID = l.LecturerID,
                        LecturerName = l.Lecturer.LastName + " " + l.Lecturer.FirstName,
                        TeachingDay = (int)l.StartTime.DayOfWeek,
                        StartTime = l.StartTime.TimeOfDay,
                        EndTime = l.StartTime.TimeOfDay.Add(
                            TimeSpan.FromMinutes((double)(l.SyllabusSchedule.DurationMinutes ?? 0))
                        )
                    })
                    .ToListAsync();

                var distinctResult = result
                    .DistinctBy(x => new { x.LecturerID, x.TeachingDay, x.StartTime, x.EndTime })
                    .ToList();

                return OperationResult<List<TeachingScheduleDTO>>.Ok(distinctResult, OperationMessages.RetrieveSuccess("lịch giảng dạy"));
            }
            catch (Exception ex)
            {
                return OperationResult<List<TeachingScheduleDTO>>.Fail($"Lỗi khi truy xuất lịch giảng dạy: {ex.Message}");
            }
        }
        public async Task<OperationResult<List<TeachingScheduleDetailDTO>>> GetTeachingScheduleDetailByID(string accountID)
        {
            try
            {
                var now = DateTime.Now;

                var result = await _dbContext.Lesson
                    .Where(l => l.IsActive &&
                                l.LecturerID == accountID &&
                                l.StartTime >= now)
                    .Select(l => new TeachingScheduleDetailDTO
                    {
                        TeachingDay = DateOnly.FromDateTime(l.StartTime),
                        StartTime = l.StartTime.TimeOfDay,
                        EndTime = l.StartTime.TimeOfDay.Add(
                            TimeSpan.FromMinutes((double)(l.SyllabusSchedule.DurationMinutes ?? 0))
                        )
                    })
                    .OrderBy(x => x.TeachingDay)
                    .ThenBy(x => x.StartTime)
                    .ToListAsync();

                return OperationResult<List<TeachingScheduleDetailDTO>>.Ok(
                    result,
                    OperationMessages.RetrieveSuccess("lịch dạy chi tiết")
                );
            }
            catch (Exception ex)
            {
                return OperationResult<List<TeachingScheduleDetailDTO>>.Fail(
                    $"Lỗi khi truy xuất lịch dạy chi tiết: {ex.Message}"
                );
            }
        }

        public async Task<OperationResult<List<AccountDTO>>> GetListAccountByRoleAsync(AccountRole accountRole)
        {
            try
            {
                var accounts = await _dbContext.Accounts
                    .Where(x => x.Role == accountRole && x.Status == AccountStatus.Active)
                    .Select(x => new AccountDTO
                    {
                        AccountID = x.AccountID,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        Gender = x.Gender.ToString(),
                        PhoneNumber = x.PhoneNumber,
                        Email = x.Email,
                        BirthDate = x.BirthDate,
                        Role = x.Role.ToString(),
                        Status = x.Status.ToString(),
                        Img = x.Image
                    })
                    .ToListAsync();

                return OperationResult<List<AccountDTO>>.Ok(
                    accounts,
                    OperationMessages.RetrieveSuccess("tài khoản")
                );
            }
            catch (Exception ex)
            {
                return OperationResult<List<AccountDTO>>.Fail($"Lỗi khi truy xuất danh sách tài khoản: {ex.Message}");
            }
        }


        public async Task<Account?> GetAccountsByIdAsync(string accountId)
        {
            return await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.AccountID == accountId);
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            var changes = await _dbContext.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<List<Account>> SearchAccountsAsync(SearchAccountsQueryCommand cmd)
        {
            var search = cmd.SearchValue?.ToLower()?.Trim();

            if (string.IsNullOrWhiteSpace(search))
                return await _dbContext.Accounts
                    .OrderBy(a => a.AccountID)
                    .Take(1000)
                    .ToListAsync();

            var allAccounts = await _dbContext.Accounts.ToListAsync();

            var matchedByName = new List<Account>();
            var matchedByOther = new List<Account>();

            if (cmd.SearchByName)
            {
                matchedByName = allAccounts
                    .Where(a => ($"{a.FirstName} {a.LastName}").ToLower().Contains(search))
                    .ToList();
            }

            if (cmd.SearchByGender || cmd.SearchByPhoneNumber || cmd.SearchByEmail || cmd.SearchByRole || cmd.SearchByStatus)
            {
                matchedByOther = allAccounts
                    .Where(a =>
                        (cmd.SearchByGender && a.Gender.ToString().ToLower().Contains(search)) ||
                        (cmd.SearchByPhoneNumber && a.PhoneNumber.ToLower().Contains(search)) ||
                        (cmd.SearchByEmail && a.Email.ToLower().Contains(search)) ||
                        (cmd.SearchByRole && a.Role.ToString().ToLower().Contains(search)) ||
                        (cmd.SearchByStatus && a.Status.ToString().ToLower().Contains(search)))
                    .ToList();
            }

            var final = matchedByName
                .Concat(matchedByOther)
                .DistinctBy(a => a.AccountID)
                .ToList();

            return final
                .OrderByDescending(a => ($"{a.FirstName} {a.LastName}").ToLower().Contains(search)) 
                .ThenBy(a => a.AccountID)
                .Take(1000)
                .ToList();
        }
        public async Task<OperationResult<List<AccountDTO>>> GetFreeLecturersAsync(CheckLecturerFreeCommand request)
        {
            var maxDuration = await _dbContext.SyllabusSchedule
                .Where(s => s.SubjectID == request.SubjectID)
                .MaxAsync(s => (int?)s.DurationMinutes) ?? 0;

            if (maxDuration == 0)
                return OperationResult<List<AccountDTO>>.Fail("Không tìm thấy thời lượng học cho môn học.");

            var numberOfLessons = await _dbContext.SyllabusSchedule
                .Where(s => s.SubjectID == request.SubjectID)
                .CountAsync();

            if (numberOfLessons == 0)
                return OperationResult<List<AccountDTO>>.Fail("Không tìm thấy nội dung giảng dạy cho môn học.");

            int lessonsPerWeek = request.dayOfWeeks.Count;
            int numberOfWeeksToCheck = (int)Math.Ceiling((double)numberOfLessons / lessonsPerWeek);

            var timeRanges = new List<(DateTime Start, DateTime End)>();

            var dateStartStart = request.DateStart;
            var dateStartEnd = dateStartStart.AddMinutes(maxDuration);
            timeRanges.Add((dateStartStart, dateStartEnd));

            for (int week = 0; week < numberOfWeeksToCheck; week++)
            {
                foreach (var dow in request.dayOfWeeks)
                {
                    int offsetDays = ((int)dow - (int)request.DateStart.DayOfWeek + 7) % 7;
                    var startDate = request.DateStart.Date.AddDays(week * 7 + offsetDays);
                    var start = startDate.Add(request.Time.ToTimeSpan());
                    var end = start.AddMinutes(maxDuration);

                    if (start != dateStartStart)
                    {
                        timeRanges.Add((start, end));
                    }
                }
            }

            var activeLessons = await _dbContext.Lesson
                .Where(l => l.IsActive)
                .Select(l => new
                {
                    l.LecturerID,
                    l.StartTime
                })
                .ToListAsync();

            var conflictingLecturerIds = activeLessons
                .Where(lesson => timeRanges.Any(t =>
                    lesson.StartTime < t.End &&
                    lesson.StartTime.AddMinutes(maxDuration) > t.Start))
                .Select(l => l.LecturerID)
                .Distinct()
                .ToList();

            var freeLecturers = await _dbContext.Accounts
                .Where(a => a.Role == AccountRole.Lecture && a.Status == AccountStatus.Active)
                .Where(a => !conflictingLecturerIds.Contains(a.AccountID))
                .Select(a => new AccountDTO
                {
                    AccountID = a.AccountID,
                    LastName = a.LastName,
                    FirstName = a.FirstName,
                    Gender = a.Gender.ToString(),
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    BirthDate = a.BirthDate,
                    Role = a.Role.ToString(),
                    Status = a.Status.ToString(),
                    Img = a.Image
                })
                .ToListAsync();

            if (!freeLecturers.Any())
                return OperationResult<List<AccountDTO>>.Fail("Không có giảng viên rảnh.");

            return OperationResult<List<AccountDTO>>.Ok(freeLecturers);
        }

    }
}
