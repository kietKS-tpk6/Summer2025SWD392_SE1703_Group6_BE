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
using Application.Common.Constants;
namespace Infrastructure.Services
{

    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILessonService _lessonService;
        private readonly ITestEventService _testEventService;
        public ClassService(IClassRepository classRepository, ISubjectRepository subjectRepository,
            IEnrollmentRepository enrollmentRepository, ILessonService lessonService, ITestEventService testEventService)
        {
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
            _enrollmentRepository = enrollmentRepository;
            _lessonService = lessonService;
            _testEventService = testEventService;
        }
        public async Task<int> GetEnrollmentCountAsync(string classId)
        {
            return await _classRepository.GetEnrollmentCountAsync(classId);
        }

        public async Task<OperationResult<string?>> CreateClassAsync(ClassCreateCommand request)
        {
            var countResult = await _classRepository.CountAsync();
            if (!countResult.Success)
                return OperationResult<string>.Fail(countResult.Message);

            var newClassId = "CL" + countResult.Data.ToString("D4");

            var newClass = new Class
            {
                ClassID = newClassId,
                LecturerID = request.LecturerID,
                SubjectID = request.SubjectID,
                ClassName = request.ClassName,
                MinStudentAcp = request.MinStudentAcp,
                MaxStudentAcp = request.MaxStudentAcp,
                PriceOfClass = request.PriceOfClass,
                Status = ClassStatus.Pending,
                CreateAt = DateTime.UtcNow,
                TeachingStartTime = request.TeachingStartTime,
                ImageURL = request.ImageURL,
            };

            return await _classRepository.CreateAsync(newClass);
        }


        public async Task<OperationResult<bool>> UpdateClassAsync(ClassUpdateCommand request)
        {
            var result = await _classRepository.GetByIdAsync(request.ClassID);
            var existing = result.Data;

            if (existing == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("Lớp học"));

            existing.LecturerID = request.LecturerID;
            existing.SubjectID = request.SubjectID;
            existing.ClassName = request.ClassName;
            existing.MinStudentAcp = request.MinStudentAcp;
            existing.MaxStudentAcp = request.MaxStudentAcp;
            existing.PriceOfClass = request.PriceOfClass;
            existing.TeachingStartTime = request.TeachingStartTime;
            existing.ImageURL = request.ImageURL;
            existing.Status = request.Status;

            return await _classRepository.UpdateAsync(existing);
        }
        public async Task<OperationResult<bool>> UpdateStatusAsync(ClassUpdateStatusCommand request)
        {
            return await _classRepository.UpdateStatusAsync(request); 
        }

        public async Task<OperationResult<bool>> DeleteClassAsync(string classId)
        {
            var classFound = await GetClassDTOByIDAsync(classId);
            if(!classFound.Success || classFound.Data == null)
            {
                return OperationResult<bool>.Fail(classFound.Message?? OperationMessages.NotFound("lớp học"));
            }
            if(classFound.Data.Status != ClassStatus.Pending)
            {
                return OperationResult<bool>.Fail(OperationMessages.DeleteFail("lớp học"));
            }
            await _lessonService.DeleteLessonByClassIDAsync(classId);
            await _testEventService.DeleteTestEventsByClassIDAsync(classId);
            return await _classRepository.DeleteAsync(classId);
            

        }

        public async Task<OperationResult<PagedResult<ClassDTO>>> GetListAsync(int page, int pageSize)
        {
            var operationResult = await _classRepository.GetPaginatedListAsync(page, pageSize);

            if (!operationResult.Success)
            {
                return OperationResult<PagedResult<ClassDTO>>.Fail(operationResult.Message);
            }
            var (items, total) = operationResult.Data;

            var result = new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };

            return OperationResult<PagedResult<ClassDTO>>.Ok(result, OperationMessages.RetrieveSuccess("danh sách lớp học"));
        }


        public async Task<OperationResult<PagedResult<ClassDTO>>> GetListBySubjectAsyn(string subjectId, int page, int pageSize)
        {
            var operationResult = await _classRepository.GetPaginatedListBySubjectAsyn(subjectId, page, pageSize);

            if (!operationResult.Success)
                return OperationResult<PagedResult<ClassDTO>>.Fail(operationResult.Message);

            var (items, total) = operationResult.Data;

            var result = new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
            return OperationResult<PagedResult<ClassDTO>>.Ok(result, OperationMessages.RetrieveSuccess("lớp học theo môn học"));
        }

        public async Task<OperationResult<PagedResult<ClassDTO>>> GetListByTeacherAsync(string teacherId, int page, int pageSize)
        {
            var operationResult = await _classRepository.GetPaginatedListByTeacherAsync(teacherId, page, pageSize);

            if (!operationResult.Success)
                return OperationResult<PagedResult<ClassDTO>>.Fail(operationResult.Message);

            var (items, total) = operationResult.Data;

            var result = new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
            return OperationResult<PagedResult<ClassDTO>>.Ok(result, OperationMessages.RetrieveSuccess("lớp học theo giáo viên"));
        }

        public async Task<OperationResult<PagedResult<ClassDTO>>> GetListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize)
        {
            var operationResult = await _classRepository.GetPaginatedListBySubjectAndTeacherAsync(subjectId, teacherId, page, pageSize);

            if (!operationResult.Success)
                return OperationResult<PagedResult<ClassDTO>>.Fail(operationResult.Message);

            var (items, total) = operationResult.Data;

            var result = new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
            return OperationResult<PagedResult<ClassDTO>>.Ok(result, OperationMessages.RetrieveSuccess("lớp học theo môn học và giáo viên"));
        }

        public async Task<OperationResult<PagedResult<ClassDTO>>> GetListByStatusAsync(string status, int page, int pageSize)
        {
            var operationResult = await _classRepository.GetPaginatedListByStatusAsync(status, page, pageSize);

            if (!operationResult.Success)
                return OperationResult<PagedResult<ClassDTO>>.Fail(operationResult.Message);

            var (items, total) = operationResult.Data;

            var result = new PagedResult<ClassDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
            return OperationResult<PagedResult<ClassDTO>>.Ok(result, OperationMessages.RetrieveSuccess("lớp học theo trạng thái"));
        }


        public async Task<OperationResult<List<ClassDTO>>> SearchClassAsync(string keyword)
        {
            var result = await _classRepository.SearchClassAsync(keyword);

            if (!result.Success)
                return OperationResult<List<ClassDTO>>.Fail(result.Message);

            return OperationResult<List<ClassDTO>>.Ok(result.Data, OperationMessages.RetrieveSuccess("kết quả tìm kiếm lớp học"));
        }


        public async Task<OperationResult<ClassDTO?>> GetClassDTOByIDAsync(string classId)
        {
            var result = await _classRepository.GetClassDTOByIdAsync(classId);

            if (!result.Success || result.Data == null)
                return OperationResult<ClassDTO?>.Fail(OperationMessages.NotFound("Lớp học"));

            return OperationResult<ClassDTO?>.Ok(result.Data, OperationMessages.RetrieveSuccess("lớp học"));
        }
        public async Task<OperationResult<ClassCreateLessonDTO>> GetClassCreateLessonDTOByIdAsync(string classId)
        {
            var result = await _classRepository.GetClassCreateLessonDTOByIdAsync(classId);

            if (result == null)
                return OperationResult<ClassCreateLessonDTO>.Fail(OperationMessages.NotFound("Lớp học"));

            return OperationResult<ClassCreateLessonDTO>.Ok(result, OperationMessages.RetrieveSuccess("lớp học để tạo buổi học"));
        }
        public async Task<OperationResult<List<StudentDTO>>> GetStudentsByClassIdAsync(string classId)
        {
            var classFound = await _classRepository.GetByIdAsync(classId);
            if (!classFound.Success || classFound.Data == null)
            {
                return OperationResult<List<StudentDTO>>.Fail(OperationMessages.NotFound("lớp học"));
            }
            return await _classRepository.GetStudentsByClassIdAsync(classId);
        }
        public async Task<OperationResult<bool>> IsClassNameDuplicateAsync(string className)
        {
            return await _classRepository.IsClassNameDuplicateAsync(className);
        }
    }

}
