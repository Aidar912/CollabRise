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
    public class DTaskStatusesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DTaskStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DTaskStatuses
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTaskStatuses.ToListAsync());
        }

        // GET: DTaskStatuses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskStatus = await _context.DTaskStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskStatus == null)
            {
                return NotFound();
            }

            return View(dtaskStatus);
        }

        // GET: DTaskStatuses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DTaskStatuses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DTaskStatus dtaskStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dtaskStatus);
        }

        // GET: DTaskStatuses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskStatus = await _context.DTaskStatuses.FindAsync(id);
            if (dtaskStatus == null)
            {
                return NotFound();
            }
            return View(dtaskStatus);
        }

        // POST: DTaskStatuses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DTaskStatus dtaskStatus)
        {
            if (id != dtaskStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtaskStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskStatusExists(dtaskStatus.Id))
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
            return View(dtaskStatus);
        }

        // GET: DTaskStatuses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskStatus = await _context.DTaskStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskStatus == null)
            {
                return NotFound();
            }

            return View(dtaskStatus);
        }

        // POST: DTaskStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskStatus = await _context.DTaskStatuses.FindAsync(id);
            _context.DTaskStatuses.Remove(dtaskStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskStatusExists(int id)
        {
            return _context.DTaskStatuses.Any(e => e.Id == id);
        }
    }
}
