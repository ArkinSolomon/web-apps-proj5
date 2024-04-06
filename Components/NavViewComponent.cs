using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAppsProject5.Models;

namespace WebAppsProject5.Components;

public class NavViewComponent(UserManager<PlannerUser> userManager) : ViewComponent
{
    [Authorize(Roles = "faculty, admin")]
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View("_NavPartial", new NavModel
        {
            Name =
                (await userManager.GetUserAsync((User as ClaimsPrincipal)!))?.Name ?? "<unknown>",
            IsAdmin = User.IsInRole("admin")
        });
    }
}