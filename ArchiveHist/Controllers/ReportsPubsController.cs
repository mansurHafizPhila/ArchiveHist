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
        public async Task<IActionResult> Index()
        {
            var archiveContext = _context.ReportsPubs.Include(r => r.CIdNavigation);
            return View(await archiveContext.ToListAsync());
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
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
