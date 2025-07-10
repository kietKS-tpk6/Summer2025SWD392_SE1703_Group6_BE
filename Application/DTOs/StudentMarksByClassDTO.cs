using Application.DTOs;

public class StudentMarksByClassDTO
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public List<StudentMarkDetailDTO> Marks { get; set; } = new List<StudentMarkDetailDTO>();
}