using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;

namespace WebAppsProject5.Models;

public class Course
{
    [Column(TypeName = "Varchar(10)")] public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required float Credits { get; init; }
    public bool IsGenEd { get; init; }
}