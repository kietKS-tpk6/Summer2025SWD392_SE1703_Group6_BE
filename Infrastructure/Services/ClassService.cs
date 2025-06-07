using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Application.DTOs;
namespace Infrastructure.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<bool> CreateClassAsync(ClassCreateCommand classCreateCommand)
        {
            var numberOfClasses = await _classRepository.CountAsync();
            string newClassID = "CL" + numberOfClasses.ToString("D4"); 

            var newClass = new Class
            {
                ClassID = newClassID,
                LecturerID = classCreateCommand.LecturerID,
                SubjectID = classCreateCommand.SubjectID,
                ClassName = classCreateCommand.ClassName,
                MinStudentAcp = classCreateCommand.MinStudentAcp,
                MaxStudentAcp = classCreateCommand.MaxStudentAcp,
                PriceOfClass = classCreateCommand.PriceOfClass,
                Status = ClassStatus.Pending,
                CreateAt = DateTime.UtcNow,
                TeachingStartTime = classCreateCommand.TeachingStartTime,
                ImageURL = classCreateCommand.ImageURL,
            };

            return await _classRepository.CreateAsync(newClass);
        }
        public async Task<bool> UpdateClassAsync(ClassUpdateCommand classUpdateCommand)
        {
            var existingClass = await _classRepository.GetByIdAsync(classUpdateCommand.ClassID);
            if (existingClass == null)
            {
                return false;
            }

            existingClass.LecturerID = classUpdateCommand.LecturerID;
            existingClass.SubjectID = classUpdateCommand.SubjectID;
            existingClass.ClassName = classUpdateCommand.ClassName;
            existingClass.MinStudentAcp = classUpdateCommand.MinStudentAcp;
            existingClass.MaxStudentAcp = classUpdateCommand.MaxStudentAcp;
            existingClass.PriceOfClass = classUpdateCommand.PriceOfClass;
            existingClass.TeachingStartTime = classUpdateCommand.TeachingStartTime;
            existingClass.ImageURL = classUpdateCommand.ImageURL;
            existingClass.Status = classUpdateCommand.Status;

            return await _classRepository.UpdateAsync(existingClass);
        }
        public async Task<PagedResult<ClassDTO>> GetPaginatedListAsync(int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListAsync(page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<(List<ClassDTO> Items, int TotalCount)> GetListBySubjectIdAsync(string subjectId, int page, int pageSize)
        {
            return await _classRepository.GetPaginatedListBySubjectAsyn(subjectId, page,pageSize);
        }

        public async Task<bool> DeleteClassAsync(string id)
        {
            return await _classRepository.DeleteAsync(id);
        }
        public async Task<PagedResult<ClassDTO>> GetListAsync(int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListAsync(page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        public async Task<PagedResult<ClassDTO>> GetListBySubjectAsyn(string subjectId, int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListBySubjectAsyn(subjectId, page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ClassDTO>> GetListByTeacherAsync(string teacherId, int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListByTeacherAsync(teacherId, page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ClassDTO>> GetListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListBySubjectAndTeacherAsync(subjectId, teacherId, page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        public async Task<PagedResult<ClassDTO>> GetListByStatusAsync(string status, int page, int pageSize)
        {
            var (items, total) = await _classRepository.GetPaginatedListByStatusAsync(status, page, pageSize);
            return new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<List<ClassDTO>> SearchClassAsync(string keyword)
        {
            return await _classRepository.SearchClassAsync(keyword);
        }
        public async Task<ClassCreateLessonDTO?> GetClassCreateLessonDTOByIdAsync(string classId)
        {
            return await _classRepository.GetClassCreateLessonDTOByIdAsync(classId);
        }

    }
}
