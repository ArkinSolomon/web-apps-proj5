using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAppsProject5.Models;

[Keyless]
public class CourseOffered
{
    public virtual Course Course { get; init; }
    [Column(TypeName = "Year")] public required int YearOffered { get; init; }
}