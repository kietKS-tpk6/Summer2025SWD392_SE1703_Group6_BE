public class UpdateSubjectDTO
{
    public string SubjectID { get; set; }
    public string SubjectName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public float MinAverageScoreToPass { get; set; }
}