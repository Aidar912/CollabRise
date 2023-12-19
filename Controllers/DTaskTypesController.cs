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
    public class DTaskTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DTaskTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DTaskTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTaskTypes.ToListAsync());
        }

        // GET: DTaskTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskType = await _context.DTaskTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskType == null)
            {
                return NotFound();
            }

            return View(dtaskType);
        }

        // GET: DTaskTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DTaskTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DTaskType dtaskType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dtaskType);
        }

        // GET: DTaskTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskType = await _context.DTaskTypes.FindAsync(id);
            if (dtaskType == null)
            {
                return NotFound();
            }
            return View(dtaskType);
        }

        // POST: DTaskTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DTaskType dtaskType)
        {
            if (id != dtaskType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtaskType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskTypeExists(dtaskType.Id))
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
            return View(dtaskType);
        }

        // GET: DTaskTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskType = await _context.DTaskTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskType == null)
            {
                return NotFound();
            }

            return View(dtaskType);
        }

        // POST: DTaskTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskType = await _context.DTaskTypes.FindAsync(id);
            _context.DTaskTypes.Remove(dtaskType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskTypeExists(int id)
        {
            return _context.DTaskTypes.Any(e => e.Id == id);
        }
    }
}
