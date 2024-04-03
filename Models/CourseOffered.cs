using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppsProject5.Models;

public class CourseOffered
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string CourseId { get; set; }
    public virtual Course? Course { get; set; }

    [Column(TypeName = "Year")]
    [DisplayName("Year Offered")]
    public required int YearOffered { get; set; }
}