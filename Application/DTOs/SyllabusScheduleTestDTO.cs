
namespace Application.DTOs
{
    public class SyllabusScheduleTestDTO
    {
        public int ID { get; set; }
        public string SyllabusSchedulesID { get; set; }
        public string TestCategory { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
