using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Data;

namespace WebAppsProject5.Models;

[Keyless]
public class Requirement
{
    public RequirementType Type { get; set; }

    [Required] public virtual Accomplishment Accomplishment { get; set; }

    [Required] public virtual Course Course { get; set; }
}