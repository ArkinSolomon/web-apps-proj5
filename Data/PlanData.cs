using WebAppsProject5.Models;

namespace WebAppsProject5.Data;

public class PlanData(
    int id,
    string name,
    Dictionary<int, string> majors,
    Dictionary<int, string> minors,
    int catalogYear)
{
    public int Id { get; private init; } = id;
    public string Name { get; private init; } = name;
    public Dictionary<int, string> Majors { get; private init; } = majors;
    public Dictionary<int, string> Minors { get; private init; } = minors;
    public int CatalogYear { get; private init; } = catalogYear;

    public static PlanData FromPlan(Plan plan)
    {
        return new PlanData(plan.Id, plan.PlanName,
            plan.Accomplishments.Where(a => a.Type == AccomplishmentType.Major).ToDictionary(a => a.Id, a => a.Name),
            plan.Accomplishments.Where(a => a.Type == AccomplishmentType.Minor).ToDictionary(a => a.Id, a => a.Name),
            plan.CatalogYear);
    }
}