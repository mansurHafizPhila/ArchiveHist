using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Drawing.Printing;

namespace ArchiveHist.Controllers
{
    public class PoisonBooksController : Controller
    {
        private readonly ArchiveContext _context;

        public PoisonBooksController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: PoisonBooks
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;

            var allRecords = await _context.PoisonBooks.Include(p => p.CIdNavigation).ToListAsync();

            ViewBag.TotalCount = allRecords.Count;

            // Calculate total pages
            ViewBag.TotalPages = pageSizeValue == -1
                ? 1
                : (int)Math.Ceiling((double)ViewBag.TotalCount / pageSizeValue);

            // Apply pagination only if not showing all
            if (pageSizeValue != -1)
            {
                allRecords = allRecords
                                   .Skip((pageNumberValue - 1) * pageSizeValue)
                                   .Take(pageSizeValue)
                                   .ToList();
            }

            return View(allRecords);
        }

        // GET: PoisonBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (poisonBook == null)
            {
                return NotFound();
            }

            return View(poisonBook);
        }

        // GET: PoisonBooks/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: PoisonBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PId,Title,Location,Author,Date,Poison,ArsenicWarning,CId")] PoisonBook poisonBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poisonBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // GET: PoisonBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks.FindAsync(id);
            if (poisonBook == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // POST: PoisonBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PId,Title,Location,Author,Date,Poison,ArsenicWarning,CId")] PoisonBook poisonBook)
        {
            if (id != poisonBook.PId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poisonBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoisonBookExists(poisonBook.PId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // GET: PoisonBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (poisonBook == null)
            {
                return NotFound();
            }

            return View(poisonBook);
        }

        // POST: PoisonBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poisonBook = await _context.PoisonBooks.FindAsync(id);
            if (poisonBook != null)
            {
                _context.PoisonBooks.Remove(poisonBook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoisonBookExists(int id)
        {
            return _context.PoisonBooks.Any(e => e.PId == id);
        }
    }
}
