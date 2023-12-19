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
    public class DTaskCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DTaskCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DTaskComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DTaskComments.Include(t => t.dtask).Include(t => t.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DTaskComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskComment = await _context.DTaskComments
                .Include(t => t.dtask)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskComment == null)
            {
                return NotFound();
            }

            return View(dtaskComment);
        }

        // GET: DTaskComments/Create
        public IActionResult Create()
        {
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DTaskComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comment,Created,UserId,DTaskId")] DTaskComment dtaskComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskComment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskComment.UserId);
            return View(dtaskComment);
        }

        // GET: DTaskComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskComment = await _context.DTaskComments.FindAsync(id);
            if (dtaskComment == null)
            {
                return NotFound();
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskComment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskComment.UserId);
            return View(dtaskComment);
        }

        // POST: DTaskComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,Created,UserId,DTaskId")] DTaskComment dtaskComment)
        {
            if (id != dtaskComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtaskComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskCommentExists(dtaskComment.Id))
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
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskComment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskComment.UserId);
            return View(dtaskComment);
        }

        // GET: DTaskComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskComment = await _context.DTaskComments
                .Include(t => t.dtask)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskComment == null)
            {
                return NotFound();
            }

            return View(dtaskComment);
        }

        // POST: DTaskComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskComment = await _context.DTaskComments.FindAsync(id);
            _context.DTaskComments.Remove(dtaskComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskCommentExists(int id)
        {
            return _context.DTaskComments.Any(e => e.Id == id);
        }
    }
}
