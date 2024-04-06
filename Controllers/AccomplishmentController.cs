using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers;

[Authorize(Roles = "admin")]
public class AccomplishmentController(ApplicationContext context) : Controller
{
    // GET: Accomplishment
    public async Task<IActionResult> Index()
    {
        return View(await context.Accomplishments.ToListAsync());
    }

    // GET: Accomplishment/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var accomplishment = await context.Accomplishments
            .FirstOrDefaultAsync(m => m.Id == id);
        if (accomplishment == null)
        {
            return NotFound();
        }

        return View(accomplishment);
    }

    // GET: Accomplishment/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Accomplishment/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Type")] Accomplishment accomplishment)
    {
        if (ModelState.IsValid)
        {
            context.Add(accomplishment);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(accomplishment);
    }

    // GET: Accomplishment/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var accomplishment = await context.Accomplishments.FindAsync(id);
        if (accomplishment == null)
        {
            return NotFound();
        }
        return View(accomplishment);
    }

    // POST: Accomplishment/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type")] Accomplishment accomplishment)
    {
        if (id != accomplishment.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(accomplishment);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccomplishmentExists(accomplishment.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(accomplishment);
    }

    // GET: Accomplishment/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var accomplishment = await context.Accomplishments
            .FirstOrDefaultAsync(m => m.Id == id);
        if (accomplishment == null)
        {
            return NotFound();
        }

        return View(accomplishment);
    }

    // POST: Accomplishment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var accomplishment = await context.Accomplishments.FindAsync(id);
        if (accomplishment != null)
        {
            context.Accomplishments.Remove(accomplishment);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AccomplishmentExists(int id)
    {
        return context.Accomplishments.Any(e => e.Id == id);
    }
}