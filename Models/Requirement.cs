using System.ComponentModel;
using WebAppsProject5.Data;

namespace WebAppsProject5.Models;

public class Requirement
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public RequirementType Type { get; set; }

    [DisplayName("Accomplishment")]
    public int AccomplishmentId { get; set; }
    public virtual Accomplishment? Accomplishment { get; set; }
    
    [DisplayName("Course")]
    public string CourseId { get; set; }
    public virtual Course? Course { get; set; }
}