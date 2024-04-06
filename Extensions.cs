using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5;

public static class Extensions
{
    public static async Task<bool> IsAuthorizedToAccess(this ApplicationContext context, string? id,
        ClaimsPrincipal loggedInPrincipal, PlannerUser loggedInUser)
    {
        var isAdmin = loggedInPrincipal.IsInRole("admin");
        var isStudent = loggedInPrincipal.IsInRole("student");

        if (isStudent && (id == null || id == loggedInUser.Id))
        {
            return true;
        }

        if (id == null && !isStudent)
        {
            return false;
        }

        if (isAdmin)
        {
            return true;
        }

        var hasThisStudent =
            await context.FacultyStudentAssignments
                .Where(fsa => fsa.FacultyId == loggedInUser.Id && fsa.StudentId == id).CountAsync() == 1;
        
        return hasThisStudent;
    }
}