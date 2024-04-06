namespace WebAppsProject5.Models;

public class PlannerModel
{
    public PlannerUser User { get; init; }
    public Plan? ActivePlan { get; init; }
    public List<PlanAccomplishment> PlanAccomplishmentMajors { get; init; }
    public bool IsStudent { get; init; }
    public string? UseIdSuffix { get; init; }
}