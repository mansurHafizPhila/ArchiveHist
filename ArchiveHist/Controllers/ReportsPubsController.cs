using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;

namespace ArchiveHist.Controllers
{
    public class ReportsPubsController : Controller
    {
        private readonly ArchiveContext _context;

        public ReportsPubsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: ReportsPubs
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;

            var allRecords = await _context.ReportsPubs.Include(r => r.CIdNavigation).ToListAsync();

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

        // GET: ReportsPubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsPub = await _context.ReportsPubs
                .Include(r => r.CIdNavigation)
                .FirstOrDefaultAsync(m => m.RpId == id);
            if (reportsPub == null)
            {
                return NotFound();
            }

            return View(reportsPub);
        }

        // GET: ReportsPubs/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 6);
            var reports = new ReportsPub { CId = 6 };
            return View(reports);
        }

        // POST: ReportsPubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RpId,Title,TotalInSeriesCopies,AgencyAuthorS,Date,Tags,Location,Notes,CId")] ReportsPub reportsPub)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reportsPub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", reportsPub.CId);
            return View(reportsPub);
        }

        // GET: ReportsPubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsPub = await _context.ReportsPubs.FindAsync(id);
            if (reportsPub == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", reportsPub.CId);
            return View(reportsPub);
        }

        // POST: ReportsPubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RpId,Title,TotalInSeriesCopies,AgencyAuthorS,Date,Tags,Location,Notes,CId")] ReportsPub reportsPub)
        {
            if (id != reportsPub.RpId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportsPub);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportsPubExists(reportsPub.RpId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", reportsPub.CId);
            return View(reportsPub);
        }

        // GET: ReportsPubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportsPub = await _context.ReportsPubs
                .Include(r => r.CIdNavigation)
                .FirstOrDefaultAsync(m => m.RpId == id);
            if (reportsPub == null)
            {
                return NotFound();
            }

            return View(reportsPub);
        }

        // POST: ReportsPubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reportsPub = await _context.ReportsPubs.FindAsync(id);
            if (reportsPub != null)
            {
                _context.ReportsPubs.Remove(reportsPub);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportsPubExists(int id)
        {
            return _context.ReportsPubs.Any(e => e.RpId == id);
        }
    }
}
