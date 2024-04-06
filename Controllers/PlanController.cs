using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAppsProject5.Data;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers;

public class PlanController(ApplicationContext context, UserManager<PlannerUser> userManager) : Controller
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> NewPlan(string? id)
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

        var newPlan = new Plan
        {
            CatalogYear = 2024,
            PlannerUser = user,
            PlannerUserId = user.Id,
            IsActive = true
        };
        context.Plans.Add(newPlan);
        var currentPlan =
            await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive).FirstOrDefaultAsync();
        if (currentPlan != null)
        {
            currentPlan.IsActive = false;
        }

        await context.SaveChangesAsync();

        return RedirectToAction("Index", "Planner", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LoadPlan(string? id)
    {
        string? selectedPlanIdStr = HttpContext.Request.Form["plan-select"];
        if (selectedPlanIdStr == null)
        {
            return BadRequest();
        }

        if (!int.TryParse(selectedPlanIdStr, out var selectedPlanId))
        {
            return BadRequest();
        }

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

        var plan = context.Plans.FirstOrDefault(p => p.Id == selectedPlanId && p.PlannerUserId == user.Id);
        if (plan == null)
        {
            return BadRequest();
        }

        plan.IsActive = true;
        var currentPlan =
            await context.Plans.Where(p => p.PlannerUserId == user.Id && p.IsActive).FirstOrDefaultAsync();
        if (currentPlan != null)
        {
            currentPlan.IsActive = false;
        }

        await context.SaveChangesAsync();

        return RedirectToAction("Index", "Planner", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdatePlan(string? id)
    {
        string? planIdStr = HttpContext.Request.Form["plan-id"];
        string? planName = HttpContext.Request.Form["plan-name"];
        string? majorsStr = HttpContext.Request.Form["majors"];
        string? minorsStr = HttpContext.Request.Form["minors"];
        if (planIdStr == null || planName == null || majorsStr == null || minorsStr == null)
        {
            return BadRequest();
        }

        if (!int.TryParse(planIdStr, out var selectedPlanId))
        {
            return BadRequest();
        }

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

        var plan = context.Plans.FirstOrDefault(p => p.Id == selectedPlanId && p.PlannerUserId == user.Id);
        if (plan == null)
        {
            return BadRequest();
        }

        var failed = false;
        var splitMajors = majorsStr.Split(',');
        var splitMinors = minorsStr.Split(',');
        var intSelector = delegate(string m)
        {
            if (failed)
            {
                return 0;
            }

            failed = !int.TryParse(m, out var parsed);
            return failed ? 0 : parsed;
        };

        var majors = majorsStr.IsNullOrEmpty() ? [] : splitMajors.Select(intSelector).ToHashSet();
        var minors = minorsStr.IsNullOrEmpty() ? [] : splitMinors.Select(intSelector).ToHashSet();
        if (failed || (majors.Count != splitMajors.Length && !majorsStr.IsNullOrEmpty()) ||
            (minors.Count != splitMinors.Length && !minorsStr.IsNullOrEmpty()))
        {
            return BadRequest();
        }

        await context.PlanAccomplishments.Where(pa => pa.PlanId == selectedPlanId).ExecuteDeleteAsync();
        var selectedAccomplishments =
            context.Accomplishments.Where(a => majors.Contains(a.Id) || minors.Contains(a.Id)).ToList();
        foreach (var selectedAccomplishment in selectedAccomplishments)
        {
            var newPlanAccomplishment = new PlanAccomplishment
            {
                AccomplishmentId = selectedAccomplishment.Id,
                PlanId = selectedPlanId
            };
            await context.PlanAccomplishments.AddAsync(newPlanAccomplishment);
        }

        plan.PlanName = planName;
        await context.SaveChangesAsync();

        return RedirectToAction("Index", "Planner", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddCourse(string? id)
    {
        string? planIdStr = HttpContext.Request.Form["plan-id"];
        string? courseId = HttpContext.Request.Form["course-id"];
        string? yearStr = HttpContext.Request.Form["year"];
        string? termStr = HttpContext.Request.Form["term"];

        if (planIdStr == null || courseId == null || yearStr == null || termStr == null)
        {
            return BadRequest();
        }

        if (!int.TryParse(yearStr, out var year) || !int.TryParse(planIdStr, out var planId) ||
            !int.TryParse(termStr, out var termNum))
        {
            return BadRequest();
        }

        var term = (TermSeason)termNum;
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

        var plan = context.Plans.FirstOrDefault(p => p.Id == planId && p.PlannerUserId == user.Id);
        if (plan == null)
        {
            return BadRequest();
        }

        await context.PlannedCourses.Where(pc => pc.PlanId == plan.Id && pc.CourseId == courseId).ExecuteDeleteAsync();

        var newPlannedCourse = new PlannedCourse
        {
            PlanId = plan.Id,
            CourseId = courseId,
            Year = year,
            TermSeason = term
        };
        plan.PlannedCourses.Add(newPlannedCourse);
        context.PlannedCourses.Add(newPlannedCourse);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveCourse(string? id)
    {
        string? planIdStr = HttpContext.Request.Form["plan-id"];
        string? courseId = HttpContext.Request.Form["course-id"];

        if (planIdStr == null || courseId == null)
        {
            return BadRequest();
        }

        if (!int.TryParse(planIdStr, out var planId))
        {
            return BadRequest();
        }

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

        var plan = context.Plans.FirstOrDefault(p => p.Id == planId && p.PlannerUserId == user.Id);
        if (plan == null)
        {
            return BadRequest();
        }

        await context.PlannedCourses.Where(pc => pc.PlanId == plan.Id && pc.CourseId == courseId).ExecuteDeleteAsync();
        await context.SaveChangesAsync();

        return NoContent();
    }
}