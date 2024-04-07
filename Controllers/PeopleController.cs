using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers;

public class PeopleController(
    ApplicationContext context,
    UserManager<PlannerUser> userManager) : Controller
{
    [Authorize(Roles = "admin, faculty")]
    public async Task<IActionResult> Students()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Logout", "Planner");
        }

        if (User.IsInRole("admin"))
        {
            var allStudents = await userManager.GetUsersInRoleAsync("student");
            List<PlannerUser> reloadedStudents = [];
            foreach (var student in allStudents)
            {
                // Eager loading requires re-querying
                reloadedStudents.Add(await context.Users.Where(s => s.Id == student.Id)
                    .Include(s => s.AdvisorAssignment)
                    .ThenInclude(fsa => fsa!.Faculty)
                    .FirstAsync());
            }

            return View(new StudentsViewModel
            {
                IsAdmin = true,
                Students = reloadedStudents
            });
        }

        var students = await context.FacultyStudentAssignments.Where(fsa => fsa.FacultyId == user.Id)
            .Include(fsa => fsa.Student)
            .ThenInclude(s => s.AdvisorAssignment)
            .Select(fsa => fsa.Student)
            .ToListAsync();

        return View(new StudentsViewModel
        {
            IsAdmin = false,
            Students = students
        });
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Faculty()
    {
        return View(await userManager.GetUsersInRoleAsync("faculty"));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ChangeFaculty(string? id)
    {
        var student = (await userManager.GetUsersInRoleAsync("student")).FirstOrDefault(u => u.Id == id);
        if (student == null)
        {
            return RedirectToAction("Students");
        }

        var faculty = await userManager.GetUsersInRoleAsync("faculty");
        var facultyList = new SelectList(faculty, "Id", "Name", student.AdvisorAssignment?.FacultyId);
        
        return View(new AssignFacultyViewModel
        {
            Student = student,
            FacultyList = facultyList
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ChangeFaculty(
        [Bind("AssignedFacultyId,StudentId")] AssignFacultyViewModel assignmentData)
    {
        var student =
            (await userManager.GetUsersInRoleAsync("student")).FirstOrDefault(u => u.Id == assignmentData.StudentId);
        if (student == null)
        {
            return BadRequest();
        }

        var faculty =
            (await userManager.GetUsersInRoleAsync("faculty")).FirstOrDefault(u =>
                u.Id == assignmentData.AssignedFacultyId);
        if (faculty == null)
        {
            return BadRequest();
        }
        
        Console.WriteLine($"F: {assignmentData.AssignedFacultyId} S: {assignmentData.StudentId}");

        await context.FacultyStudentAssignments.Where(fsa => fsa.StudentId == assignmentData.StudentId)
            .ExecuteDeleteAsync();

        var facultyStudentAssignment = new FacultyStudentAssignment
        {
            StudentId = student.Id,
            FacultyId = faculty.Id,
        };
        await context.FacultyStudentAssignments.AddAsync(facultyStudentAssignment);
        student.AdvisorAssignment = facultyStudentAssignment;
        await context.SaveChangesAsync();

        return RedirectToAction("Students");
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PromoteUser(string? id)
    {
        var user = (await userManager.GetUsersInRoleAsync("student")).FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return BadRequest();
        }

        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PromoteUser([Bind("Id")] PlannerUser? user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        await context.FacultyStudentAssignments.Where(fsa => fsa.StudentId == user.Id)
            .ExecuteDeleteAsync();
        await userManager.RemoveFromRoleAsync(user, "student");
        await userManager.AddToRoleAsync(user, "faculty");
        await context.SaveChangesAsync();

        return RedirectToAction("Faculty");
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return BadRequest();
        }

        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete([Bind("Id")] PlannerUser? user)
    {
        if (user == null)
        {
            return BadRequest();
        }
        
        var id = user.Id;
        var redirect = await userManager.IsInRoleAsync(user, "faculty") ? "/People/Faculty" : "/People/Students"; 
        await context.FacultyStudentAssignments.Where(fsa => fsa.FacultyId == id || fsa.StudentId == id)
            .ExecuteDeleteAsync();
        await context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();

        return Redirect(redirect);
    }
}