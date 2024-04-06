namespace WebAppsProject5.Models;

public class StudentsViewModel
{
    public bool IsAdmin { get; init; }
    public IEnumerable<PlannerUser> Students { get; init; }
}