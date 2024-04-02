namespace WebAppsProject5.Data;

public class CurrentPlanData
{
    public required string Student { get; init; }
    public string? Name { get; init; }
    public required List<string> Majors { get; init; }
    public required List<string> Minors { get; init; }
    public required int CurrYear { get; init; }
    public required TermSeason CurrTerm { get; init; }
    public int? Id { get; init; }
    public required int CatYear { get; init; }
    public required Dictionary<string, PlannedCourseData> Courses { get; init; }
}