using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Domain.Entities;
using Domain.Enums;
namespace Infrastructure.Services
{
    public class AssessmentCriteriaService : IAssessmentCriteriaService
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        public AssessmentCriteriaService(IAssessmentCriteriaRepository assessmentCriteriaRepository) {
                _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }
        public async Task<bool> CreateAssessmentCriteriaAsync(AssessmentCriteriaCreateCommand assessmentCriteriaCreateCommand) {
            var numberOfAssCri = await _assessmentCriteriaRepository.CountAsync();
            string newAssessmentCriteriaID = "AC" + numberOfAssCri.ToString("D4"); // D5 = 5 chữ số, vd: 00001
            var newAssCri = new AssessmentCriteria();
            newAssCri.AssessmentCriteriaID = newAssessmentCriteriaID;
            newAssCri.SyllabusID = assessmentCriteriaCreateCommand.SyllabusID;
            newAssCri.WeightPercent = assessmentCriteriaCreateCommand.WeightPercent;
            newAssCri.Category = (AssessmentCategory)assessmentCriteriaCreateCommand.Category;
            newAssCri.RequiredCount = assessmentCriteriaCreateCommand.RequiredCount;
            newAssCri.Duration = assessmentCriteriaCreateCommand.Duration;
            newAssCri.TestType = (TestType)assessmentCriteriaCreateCommand.TestType;
            newAssCri.Note = assessmentCriteriaCreateCommand.Note;
            newAssCri.MinPassingScore = assessmentCriteriaCreateCommand.MinPassingScore;
            newAssCri.IsActive = true;
            return  await _assessmentCriteriaRepository.CreateAsync(newAssCri);
        }
    }
}
 