using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppsProject5.Models;

namespace WebAppsProject5.Controllers
{
    public class CourseOfferedController : Controller
    {
        private readonly ApplicationContext _context;

        public CourseOfferedController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: CourseOffered
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.CourseOfferedYears.Include(c => c.Course);
            return View(await applicationContext.ToListAsync());
        }

        // GET: CourseOffered/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseOffered = await _context.CourseOfferedYears
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
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
                _context.Add(courseOffered);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "DisplayName", courseOffered.CourseId);
            return View(courseOffered);
        }

        // GET: CourseOffered/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseOffered = await _context.CourseOfferedYears.FindAsync(id);
            if (courseOffered == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "DisplayName", courseOffered.CourseId);
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
                    _context.Update(courseOffered);
                    await _context.SaveChangesAsync();
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

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseOffered.CourseId);
            return View(courseOffered);
        }

        // GET: CourseOffered/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseOffered = await _context.CourseOfferedYears
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
            var courseOffered = await _context.CourseOfferedYears.FindAsync(id);
            if (courseOffered != null)
            {
                _context.CourseOfferedYears.Remove(courseOffered);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseOfferedExists(Guid id)
        {
            return _context.CourseOfferedYears.Any(e => e.Id == id);
        }
    }
}