using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Data;

namespace WebAppsProject5.Models;

public class PlannedCourse
{
    public int Id { get; init; }
    
    [ForeignKey("Plan")]
    public required int PlanId { get; init; }
    [IgnoreDataMember]
    public virtual Plan Plan { get; init; } = null!;
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    [ForeignKey("Course")]
    public required string CourseId { get; init; }
    [IgnoreDataMember]
    public virtual Course Course { get; init; } = null!;
    public TermSeason TermSeason { get; init; }
    [Column(TypeName = "Year")] public int Year { get; init; }
}