using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers
{
    public class RequirementController(ApplicationContext context) : Controller
    {
        // GET: Requirement
        public async Task<IActionResult> Index()
        {
            var applicationContext = context.Requirements.Include(r => r.Accomplishment).Include(r => r.Course);
            return View(await applicationContext.ToListAsync());
        }

        // GET: Requirement/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await context.Requirements
                .Include(r => r.Accomplishment)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requirement == null)
            {
                return NotFound();
            }

            return View(requirement);
        }

        // GET: Requirement/Create
        public IActionResult Create()
        {
            ViewData["AccomplishmentId"] = new SelectList(context.Accomplishments, "Id", "DisplayName");
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName");
            return View();
        }

        // POST: Requirement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,AccomplishmentId,CourseId")] Requirement requirement)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                context.Add(requirement);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccomplishmentId"] = new SelectList(context.Accomplishments, "Id", "DisplayName", requirement.AccomplishmentId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName", requirement.CourseId);
            return View(requirement);
        }

        // GET: Requirement/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await context.Requirements.FindAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }
            ViewData["AccomplishmentId"] = new SelectList(context.Accomplishments, "Id", "DisplayName", requirement.AccomplishmentId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName", requirement.CourseId);
            return View(requirement);
        }

        // POST: Requirement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Type,AccomplishmentId,CourseId")] Requirement requirement)
        {
            if (id != requirement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(requirement);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequirementExists(requirement.Id))
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
            ViewData["AccomplishmentId"] = new SelectList(context.Accomplishments, "Id", "DisplayName", requirement.AccomplishmentId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "DisplayName", requirement.CourseId);
            return View(requirement);
        }

        // GET: Requirement/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await context.Requirements
                .Include(r => r.Accomplishment)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requirement == null)
            {
                return NotFound();
            }

            return View(requirement);
        }

        // POST: Requirement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var requirement = await context.Requirements.FindAsync(id);
            if (requirement != null)
            {
                context.Requirements.Remove(requirement);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequirementExists(Guid id)
        {
            return context.Requirements.Any(e => e.Id == id);
        }
    }
}
