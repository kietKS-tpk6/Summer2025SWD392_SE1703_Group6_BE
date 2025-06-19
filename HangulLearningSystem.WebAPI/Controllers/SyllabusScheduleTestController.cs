//using Microsoft.AspNetCore.Mvc;
//using Application.Usecases.Command;
//using MediatR;
//using Application.IServices;
//using Microsoft.AspNetCore.Mvc;
//using Infrastructure.Services;
//using Domain.Enums;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using System.Threading;
//using Domain.Entities;
//namespace HangulLearningSystem.WebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SyllabusScheduleTestController : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;
//        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
//        private readonly ISyllabusScheduleService _syllabusScheduleService;


//        public SyllabusScheduleTestController(IMediator mediator, ISyllabusScheduleTestService syllabusScheduleTestService , ISyllabusScheduleService syllabusScheduleService, IAssessmentCriteriaService assessmentCriteriaService)
//        {
//            _mediator = mediator;
//            _syllabusScheduleTestService = syllabusScheduleTestService;
//            _assessmentCriteriaService = assessmentCriteriaService;
//            _syllabusScheduleService = syllabusScheduleService;
//            _assessmentCriteriaService = assessmentCriteriaService;
//        }
//        [HttpGet("check-completeness")]
//        public async Task<IActionResult> CheckCompleteness([FromQuery] string subjectID)
//        {
//            if (string.IsNullOrWhiteSpace(subjectID))
//            {
//                return BadRequest("subjectID is required.");
//            }

//            var result = await _syllabusScheduleTestService.CheckAddAssessmentCompletenessAsync(subjectID);

//            return Ok(new { message = result });
//        }
//        [HttpPost("add-test-to-slot")]
//        public async Task<IActionResult> AddTestToSlot([FromBody] AddTestSchedulesToSlotsCommand command, CancellationToken cancellationToken)
//        {
//            // Validate input chuỗi
//            if (string.IsNullOrWhiteSpace(command.SyllabusScheduleID))
//                return BadRequest("SyllabusScheduleId is required.");
//            if (string.IsNullOrWhiteSpace(command.TestCategory))
//                return BadRequest("TestCategory is required.");
//            if (string.IsNullOrWhiteSpace(command.TestType))
//                return BadRequest("TestType is required.");
//            if (string.IsNullOrWhiteSpace(command.subjectID))
//                return BadRequest("SyllabusId is required.");

//            // Bước 1: Chuẩn hóa enums
//            TestCategory? parsedCategory;
//            TestType? parsedType;

//            try
//            {
//                parsedCategory = _syllabusScheduleTestService.NormalizeTestCategory(command.TestCategory);
//                parsedType = _syllabusScheduleTestService.NormalizeTestType(command.TestType);
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(ex.Message); 
//            }

//            if (parsedCategory == null || parsedType == null)
//                return BadRequest("TestCategory or TestType is invalid.");

//            // Bước 2: Kiểm tra slot có cho phép kiểm tra không
//            var slotAllowed = await _syllabusScheduleService.slotAllowToTestAsync(command.SyllabusScheduleID);
//            if (!slotAllowed)
//                return BadRequest("Slot is not active or does not allow test.");

//            // Bước 3: Kiểm tra bài test có trong AssessmentCriteria không
//            var isDefined = await _assessmentCriteriaService.IsTestDefinedInCriteriaAsync(command.subjectID, parsedCategory.Value, parsedType.Value);
//            if (!isDefined)
//                return BadRequest("This test is not defined in the assessment criteria.");

//           // Bước 4: Kiểm tra bài test có vượt quá số lượng cho phép không
//            var isOverLimit = await _syllabusScheduleTestService.IsTestOverLimitAsync(command.subjectID, parsedCategory.Value, parsedType.Value);
//            if (isOverLimit)
//                return BadRequest("Test count exceeds the required count defined in assessment criteria.");


//            // Bước 5: Kiểm tra xem slot đó có bài kiểm tra nào chưa
//            var hasExistingTest = await _syllabusScheduleTestService.HasTestAsync(command.SyllabusScheduleID);
//            if (hasExistingTest)
//                return BadRequest("This schedule slot already has a test. Cannot add another test to the same slot.");

//            // Bước 6: Kiểm tra thứ tự test
//            var isOrderValid = await _syllabusScheduleService.ValidateTestPositionAsync(command.subjectID,command.SyllabusScheduleID, command.TestCategory);
//            if (!isOrderValid)
//                return BadRequest("Test order is invalid: Final must come after all other tests, and Midterm must appear before Final.");
            
//            var result = await _mediator.Send(command, cancellationToken);

//            return Ok(new { message = result });
//        }


//        [HttpDelete("remove-test-from-slot")]
//        public async Task<IActionResult> RemoveTestFromSlot([FromQuery] string SyllabusScheduleID)
//        {
//            if (string.IsNullOrWhiteSpace(SyllabusScheduleID))
//            {
//                return BadRequest("syllabusId is required.");
//            }

//            var result = await _syllabusScheduleTestService.RemoveTestFromSlotAsyncs(SyllabusScheduleID);

//            return Ok(new { message = result });
//        }

//        [HttpPut("update-test-from-slot")]
//        public async Task<IActionResult> UpdateTestFromSlot([FromBody] UpdateTestSchedulesToSlotsCommand command,CancellationToken cancellationToken)
//        {
//            var result = await _mediator.Send(command, cancellationToken);

//            return Ok(new { message = result });
//        }
//        [HttpGet("get-test-added")]
//        public async Task<IActionResult> GetExamAddedAsync([FromQuery] string subject)
//        {
//            var result = await _syllabusScheduleTestService.GetExamAddedAsync(subject);

//            return Ok( result );
//        }

//    }
//}
