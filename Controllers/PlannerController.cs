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
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var activePlan = await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive).FirstOrDefaultAsync();
        return View(new PlannerModel
        {
            User = user,
            ActivePlan = activePlan
        });
    }

    [Authorize]
    public async Task<IActionResult> Requirements()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        List<string> core = [];
        List<string> electives = [];
        List<string> cognates = [];
        List<string> genEds = [];

        var activePlan = await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive)
            .Include(plan => plan.Accomplishments).FirstOrDefaultAsync();
        if (activePlan != null)
        {
            foreach (var accomplishment in activePlan.Accomplishments)
            {
                var requirements = context.Requirements
                    .Where(requirement => requirement.Accomplishment!.Id == accomplishment.Id).ToList();
                if (requirements.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var requirement in requirements)
                {
                    List<string> addList;
                    switch (requirement.Type)
                    {
                        case RequirementType.Core:
                            addList = core;
                            break;
                        case RequirementType.Cognate:
                            addList = cognates;
                            break;
                        case RequirementType.Elective:
                            addList = electives;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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
    public async Task<IActionResult> Catalog()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var plannedCourses = new Dictionary<string, PlannedCourseData>();

        var activePlan = context.Plans.Include(plan => plan.Accomplishments)
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
            Majors = activePlan?.Accomplishments.Where(a => a.Type == AccomplishmentType.Major).Select(a => a.Name)
                .Order().ToList() ?? [],
            Minors = activePlan?.Accomplishments.Where(a => a.Type == AccomplishmentType.Minor).Select(a => a.Name)
                .Order().ToList() ?? []
        };

        var userPlans = context.Plans.Where(p => p.PlannerUser.Id == user.Id).AsEnumerable().Select(PlanData.FromPlan)
            .ToDictionary(p => p.Id, p => p);

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
            Plans = userPlans,
            Catalog = currentCatalog
        });
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }
}