using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;

namespace Dsana.Controllers
{
    public class DTaskPrioritiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DTaskPrioritiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DTaskPriorities
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTaskPriorities.ToListAsync());
        }

        // GET: DTaskPriorities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskPriority = await _context.DTaskPriorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskPriority == null)
            {
                return NotFound();
            }

            return View(dtaskPriority);
        }

        // GET: DTaskPriorities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DTaskPriorities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DTaskPriority dtaskPriority)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskPriority);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dtaskPriority);
        }

        // GET: DTaskPriorities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskPriority = await _context.DTaskPriorities.FindAsync(id);
            if (dtaskPriority == null)
            {
                return NotFound();
            }
            return View(dtaskPriority);
        }

        // POST: DTaskPriorities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DTaskPriority dtaskPriority)
        {
            if (id != dtaskPriority.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtaskPriority);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskPriorityExists(dtaskPriority.Id))
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
            return View(dtaskPriority);
        }

        // GET: DTaskPriorities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskPriority = await _context.DTaskPriorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskPriority == null)
            {
                return NotFound();
            }

            return View(dtaskPriority);
        }

        // POST: DTaskPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskPriority = await _context.DTaskPriorities.FindAsync(id);
            _context.DTaskPriorities.Remove(dtaskPriority);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskPriorityExists(int id)
        {
            return _context.DTaskPriorities.Any(e => e.Id == id);
        }
    }
}
