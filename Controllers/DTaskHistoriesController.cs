using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;

namespace Dsana.Controllers
{
    public class DTaskHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<DSUser> _userManager;

        public DTaskHistoriesController(ApplicationDbContext context, UserManager<DSUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DTaskHistories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DTaskHistories.Include(t => t.DTask).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DTaskHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskHistory = await _context.DTaskHistories
                .Include(t => t.DTask)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskHistory == null)
            {
                return NotFound();
            }

            return View(dtaskHistory);
        }

        // GET: DTaskHistories/Create
        public IActionResult Create()
        {
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DTaskHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DTaskId,Property,OldValue,NewValue,Created,Description,UserId")] DTaskHistory dtaskHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskHistory.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskHistory.UserId);
            return View(dtaskHistory);
        }

        // GET: DTaskHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskHistory = await _context.DTaskHistories.FindAsync(id);
            if (dtaskHistory == null)
            {
                return NotFound();
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskHistory.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskHistory.UserId);
            return View(dtaskHistory);
        }

        // POST: DTaskHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DTaskId,Property,OldValue,NewValue,Created,Description,UserId")] DTaskHistory dtaskHistory)
        {
            if (id != dtaskHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                

                try
                {
                    _context.Update(dtaskHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskHistoryExists(dtaskHistory.Id))
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
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskHistory.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskHistory.UserId);
            return View(dtaskHistory);
        }

        // GET: DTaskHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskHistory = await _context.DTaskHistories
                .Include(t => t.DTask)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskHistory == null)
            {
                return NotFound();
            }

            return View(dtaskHistory);
        }

        // POST: DTaskHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskHistory = await _context.DTaskHistories.FindAsync(id);
            _context.DTaskHistories.Remove(dtaskHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskHistoryExists(int id)
        {
            return _context.DTaskHistories.Any(e => e.Id == id);
        }
    }
}
