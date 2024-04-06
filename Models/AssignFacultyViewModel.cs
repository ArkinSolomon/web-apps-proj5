using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppsProject5.Models;

public class AssignFacultyViewModel
{
    public SelectList FacultyList { get; init; }
    public PlannerUser Student { get; init; }
    [Display(Name = "Assigned Faculty")]
    public string? AssignedFacultyId { get; init; }
    public string? StudentId { get; init; }
}