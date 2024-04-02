using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;
using WebAppsProject5.Data;

namespace WebAppsProject5.Models;

public class Accomplishment
{
    public int Id { get; init; }
    public required string Name { get; set; }
    
    public required AccomplishmentType Type { get; set; }

    [NotMapped]
    public string DisplayName => $"{Name} ({Type.Humanize()})";
}