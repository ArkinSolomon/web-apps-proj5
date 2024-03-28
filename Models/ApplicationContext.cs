using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAppsProject5.Models;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : IdentityDbContext<PlannerUser>(options);
