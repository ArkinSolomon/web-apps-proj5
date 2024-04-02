namespace WebAppsProject5.Data;

public class CurrentCatalogData
{
    public required int Year { get; init; }
    public required Dictionary<string, CourseData> Courses { get; init; }
    public required AccomplishmentsData Accomplishments { get; init; }
}