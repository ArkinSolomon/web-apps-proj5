using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAppsProject5.Data;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers;

public class PlannerController(
    ApplicationContext context,
    UserManager<PlannerUser> userManager,
    SignInManager<PlannerUser> signInManager) : Controller
{

    [Authorize]
    public async Task<IActionResult> Index(string? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null || !await context.IsAuthorizedToAccess(id, User, user))
        {
            if (id == null && user != null && !await userManager.IsInRoleAsync(user, "student"))
            {
                return RedirectToAction("Students", "People");
            }

            return RedirectToAction("Logout");
        }

        user = id == null ? user : await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var activePlan = await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive).FirstOrDefaultAsync();
        return View(new PlannerModel
        {
            User = user,
            ActivePlan = activePlan,
            PlanAccomplishmentMajors = await context.PlanAccomplishments
                .Where(pa => pa.Accomplishment.Type == AccomplishmentType.Major).Include(pa => pa.Accomplishment)
                .ToListAsync(),
            IsStudent = User.IsInRole("student"),
            UseIdSuffix = id
        });
    }

    [Authorize]
    public async Task<IActionResult> Requirements(string? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null || !await context.IsAuthorizedToAccess(id, User, user))
        {
            return Unauthorized();
        }

        user = id == null ? user : await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Unauthorized();
        }

        List<string> core = [];
        List<string> electives = [];
        List<string> cognates = [];
        List<string> genEds = [];

        var activePlan = await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive).FirstOrDefaultAsync();
        if (activePlan != null)
        {
            var planAccomplishments =
                await context.PlanAccomplishments.Where(pa => pa.PlanId == activePlan.Id).Include(pa => pa.Accomplishment).ToListAsync();
            foreach (var planAccomplishment in planAccomplishments)
            {
                var requirements = await context.Requirements
                    .Include(r => r.Accomplishment)
                    .Where(requirement => requirement.Accomplishment!.Id == planAccomplishment.Accomplishment.Id)
                    .ToListAsync();
                if (requirements.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var requirement in requirements)
                {
                    var addList = requirement.Type switch
                    {
                        RequirementType.Core => core,
                        RequirementType.Cognate => cognates,
                        RequirementType.Elective => electives,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (!addList.Contains(requirement.CourseId))
                    {
                        addList.Add(requirement.CourseId);
                    }
                }
            }

            genEds = context.Courses.Where(c => c.IsGenEd).Select(c => c.Id).ToList();
        }

        return Json(new RequirementsData
        {
            Core = core,
            Electives = electives,
            Cognates = cognates,
            GenEds = genEds
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Catalog(string? id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null || !await context.IsAuthorizedToAccess(id, User, user))
        {
            return Unauthorized();
        }

        user = id == null ? user : await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Unauthorized();
        }

        var plannedCourses = new Dictionary<string, PlannedCourseData>();

        var activePlan = context.Plans
            .Include(plan => plan.PlannedCourses)
            .ThenInclude(plannedCourse => plannedCourse.Course)
            .FirstOrDefault(p => p.PlannerUserId == user.Id && p.IsActive);
        if (activePlan != null)
        {
            foreach (var plannedCourse in activePlan.PlannedCourses)
            {
                plannedCourses[plannedCourse.Course.Id] = new PlannedCourseData
                {
                    Id = plannedCourse.Course.Id,
                    Term = plannedCourse.TermSeason,
                    Year = plannedCourse.Year
                };
            }
        }

        var currentPlan = new CurrentPlanData
        {
            Student = user.Name,
            CatYear = activePlan?.CatalogYear ?? 0,
            Name = activePlan?.PlanName,
            CurrTerm = user.CurrentTermSeason,
            CurrYear = user.Year,
            Id = activePlan?.Id,
            Courses = plannedCourses,
            Majors = activePlan != null
                ? context.PlanAccomplishments
                    .Where(pa => pa.PlanId == activePlan.Id && pa.Accomplishment.Type == AccomplishmentType.Major)
                    .Select(pa => pa.Accomplishment.Name)
                    .ToList().Order().ToList()
                : [],
            Minors = activePlan != null
                ? context.PlanAccomplishments
                    .Where(pa => pa.PlanId == activePlan.Id && pa.Accomplishment.Type == AccomplishmentType.Minor)
                    .Select(pa => pa.Accomplishment.Name)
                    .ToList().Order().ToList()
                : []
        };

        var userPlans = await context.Plans.Where(p => p.PlannerUser.Id == user.Id).ToListAsync();
        var userPlanData = userPlans.Select(p =>
            {
                var majors = context.PlanAccomplishments.Include(pa => pa.Accomplishment)
                    .Where(pa => pa.PlanId == p.Id && pa.Accomplishment.Type == AccomplishmentType.Major)
                    .ToDictionary(pa => pa.Accomplishment.Id, pa => pa.Accomplishment.Name);
                var minors = context.PlanAccomplishments.Include(pa => pa.Accomplishment)
                    .Where(pa => pa.PlanId == p.Id && pa.Accomplishment.Type == AccomplishmentType.Minor)
                    .ToDictionary(pa => pa.Accomplishment.Id, pa => pa.Accomplishment.Name);
                return new PlanData
                {
                    Id = p.Id,
                    Name = p.PlanName,
                    Majors = majors,
                    Minors = minors,
                    CatalogYear = p.CatalogYear
                };
            })
            .ToDictionary(pd => pd.Id, pd => pd);

        CurrentCatalogData? currentCatalog = null;
        if (activePlan != null)
        {
            var courses = context.CourseOfferedYears.Include(coy => coy.Course)
                .Where(coy => coy.YearOffered == activePlan.CatalogYear)
                .ToDictionary(c => c.Course!.Id, c => CourseData.FromCourse(c.Course!));

            currentCatalog = new CurrentCatalogData
            {
                Year = activePlan.CatalogYear,
                Courses = courses,
                Accomplishments = new AccomplishmentsData
                {
                    majors = context.Accomplishments.Where(a => a.Type == AccomplishmentType.Major)
                        .ToDictionary(a => a.Id, a => a.Name),
                    minors = context.Accomplishments.Where(a => a.Type == AccomplishmentType.Minor)
                        .ToDictionary(a => a.Id, a => a.Name),
                }
            };
        }

        return Json(new CatalogData
        {
            Plan = currentPlan,
            Plans = userPlanData,
            Catalog = currentCatalog
        });
    }

    public async Task<IActionResult> Logout()
    {
        try
        {
            await signInManager.SignOutAsync();
        }
        catch
        {
            // ignored
        }

        return RedirectToAction("Index");
    }
}