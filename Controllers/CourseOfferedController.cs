using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers;

[Authorize(Roles = "admin")]
public class CourseOfferedController(ApplicationContext context) : Controller
{
    // GET: CourseOffered
    public async Task<IActionResult> Index()
    {
        var coys = context.CourseOfferedYears.Include(c => c.Course).OrderBy(c => c.Id);
        return View(await coys.ToListAsync());
    }

    // GET: CourseOffered/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courseOffered = await context.CourseOfferedYears
            .Include(c => c.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (courseOffered == null)
        {
            return NotFound();
        }

        return View(courseOffered);
    }

    // GET: CourseOffered/Create
    public IActionResult Create()
    {
        ViewData["CourseId"] = new SelectList(context.Courses, "Id", "Id");
        return View();
    }

    // POST: CourseOffered/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CourseId,YearOffered")] CourseOffered courseOffered)
    {
        foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        if (ModelState.IsValid)
        {
            context.Add(courseOffered);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName", courseOffered.CourseId);
        return View(courseOffered);
    }

    // GET: CourseOffered/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courseOffered = await context.CourseOfferedYears.FindAsync(id);
        if (courseOffered == null)
        {
            return NotFound();
        }

        ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName", courseOffered.CourseId);
        return View(courseOffered);
    }

    // POST: CourseOffered/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,CourseId,YearOffered")] CourseOffered courseOffered)
    {
        if (id != courseOffered.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(courseOffered);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseOfferedExists(courseOffered.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CourseId"] = new SelectList(context.Courses, "Id", "Id", courseOffered.CourseId);
        return View(courseOffered);
    }

    // GET: CourseOffered/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courseOffered = await context.CourseOfferedYears
            .Include(c => c.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (courseOffered == null)
        {
            return NotFound();
        }

        return View(courseOffered);
    }

    // POST: CourseOffered/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var courseOffered = await context.CourseOfferedYears.FindAsync(id);
        if (courseOffered != null)
        {
            context.CourseOfferedYears.Remove(courseOffered);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseOfferedExists(Guid id)
    {
        return context.CourseOfferedYears.Any(e => e.Id == id);
    }
}