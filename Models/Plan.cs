using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppsProject5.Models;

public class Plan
{
    public int Id { get; init; }
    [Column(TypeName = "Varchar(32)")] public string PlanName { get; set; } = "New Plan";
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string PlannerUserId { get; init; }
    public virtual PlannerUser PlannerUser { get; init; } = null!;

    public List<Accomplishment> Accomplishments { get; set; } = [];
    public int CatalogYear { get; init; }
    public List<PlannedCourse> PlannedCourses { get; set; } = [];

    public bool IsActive { get; set; } = false;
}