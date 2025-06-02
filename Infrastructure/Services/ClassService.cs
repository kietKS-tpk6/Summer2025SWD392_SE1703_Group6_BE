using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
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
    }
}
