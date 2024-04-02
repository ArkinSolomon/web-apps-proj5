namespace WebAppsProject5.Data;

public class CatalogData
{
    public required CurrentPlanData Plan { get; init; }
    public required Dictionary<int, PlanData> Plans { get; init; }
    public CurrentCatalogData? Catalog { get; init; }
}