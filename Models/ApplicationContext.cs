using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAppsProject5.Models;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : IdentityDbContext<PlannerUser>(options)
{
    public DbSet<Accomplishment> Accomplishments { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlannedCourse> PlannedCourses { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<CourseOffered> CourseOfferedYears { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(true);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PlannedCourse>()
            .HasOne(pc => pc.Plan)
            .WithMany(p => p.PlannedCourses)
            .HasForeignKey(pc => pc.PlanId)
            .IsRequired();
        
        builder.Entity<PlannedCourse>()
            .HasOne(pc => pc.Course)
            .WithMany()
            .HasForeignKey(pc => pc.CourseId)
            .IsRequired();
        
        base.OnModelCreating(builder);
    }
}
