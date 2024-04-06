namespace WebAppsProject5.Models;

public class PlanAccomplishment
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public int PlanId { get; init; }
    public virtual Plan Plan { get; init; } = null!;
    public int AccomplishmentId { get; init; }
    public Accomplishment Accomplishment { get; init; } = null!;
}