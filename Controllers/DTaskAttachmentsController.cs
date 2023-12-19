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
    public class DTaskAttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DTaskAttachmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DTaskAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DTaskAttachments.Include(t => t.DTask).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DTaskAttachments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskAttachment = await _context.DTaskAttachments
                .Include(t => t.DTask)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskAttachment == null)
            {
                return NotFound();
            }

            return View(dtaskAttachment);
        }

        // GET: DTaskAttachments/Create
        public IActionResult Create()
        {
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DTaskAttachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DTaskId,Created,UserId,Description,FileName,FileData,FileContentType")] DTaskAttachment dtaskAttachment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dtaskAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskAttachment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskAttachment.UserId);
            return View(dtaskAttachment);
        }

        // GET: DTaskAttachments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskAttachment = await _context.DTaskAttachments.FindAsync(id);
            if (dtaskAttachment == null)
            {
                return NotFound();
            }
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskAttachment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskAttachment.UserId);
            return View(dtaskAttachment);
        }

        // POST: DTaskAttachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DTaskId,Created,UserId,Description,FileName,FileData,FileContentType")] DTaskAttachment dtaskAttachment)
        {
            if (id != dtaskAttachment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtaskAttachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTaskAttachmentExists(dtaskAttachment.Id))
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
            ViewData["DTaskId"] = new SelectList(_context.DTasks, "Id", "Description", dtaskAttachment.DTaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dtaskAttachment.UserId);
            return View(dtaskAttachment);
        }

        // GET: DTaskAttachments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtaskAttachment = await _context.DTaskAttachments
                .Include(t => t.DTask)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dtaskAttachment == null)
            {
                return NotFound();
            }

            return View(dtaskAttachment);
        }

        // POST: DTaskAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtaskAttachment = await _context.DTaskAttachments.FindAsync(id);
            _context.DTaskAttachments.Remove(dtaskAttachment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTaskAttachmentExists(int id)
        {
            return _context.DTaskAttachments.Any(e => e.Id == id);
        }
    }
}
