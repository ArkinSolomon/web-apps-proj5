using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppsProject5.Models;

public class FacultyStudentAssignment
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    
    public PlannerUser Faculty { get; set; }
    [ForeignKey("Faculty")] public string FacultyId { get; set; }
    
    public PlannerUser Student { get; set; }
    [ForeignKey("Student")] public string StudentId { get; set; }
}