using WebAppsProject5.Models;

namespace WebAppsProject5.Data;

public class PlanData
{
    public int Id { get; init; }
    public string Name { get; init; }
    public Dictionary<int, string> Majors { get; init; }
    public Dictionary<int, string> Minors { get; init; }
    public int CatalogYear { get; init; }
}