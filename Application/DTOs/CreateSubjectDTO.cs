﻿public class CreateSubjectDTO
{
    public string SubjectID { get; set; }
    public string SubjectName { get; set; }
    public string Description { get; set; }
    public float MinAverageScoreToPass { get; set; } = 5.0f;
}