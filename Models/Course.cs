using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppsProject5.Models;

public class Course
{
    [Column(TypeName = "Varchar(10)")]
    [StringLength(10, MinimumLength = 3, ErrorMessage = "Invalid length")]
    [RegularExpression(@"^[A-Z]{2,5}-\d{3,4}$", ErrorMessage = "Invalid course identifier format")]
    [ReadOnly(true)]
    [DisplayName("Course Id")]
    public required string Id { get; init; }

    [Column(TypeName = "Varchar(64)")]
    [StringLength(64, MinimumLength = 3, ErrorMessage = "Invalid length")]
    [DisplayName("Course Name")]
    public required string Name { get; set; }
    
    [DisplayName("Course Description")]
    public required string Description { get; set; }
    
    public required float Credits { get; init; }
    
    [DisplayName("Is Gen-Ed")]
    public bool IsGenEd { get; init; }

    [NotMapped] public string DisplayName => $"{Id} {Name}";
}