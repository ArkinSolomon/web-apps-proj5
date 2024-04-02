namespace WebAppsProject5.Models;

public class PlannerModel
{
    public PlannerUser User { get; init; }
    public Plan? ActivePlan { get; init; }
}