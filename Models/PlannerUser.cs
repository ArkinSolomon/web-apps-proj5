using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebAppsProject5.Data;

namespace WebAppsProject5.Models;

public class PlannerUser : IdentityUser
{
    [Column(TypeName = "Year")] public int Year { get; init; } = 2024;
    public TermSeason CurrentTermSeason { get; init; } = TermSeason.Spring;
    
    [Column(TypeName = "Varchar(33)")] public string Name { get; set; }
}