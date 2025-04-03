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
    public class TrunksController : Controller
    {
        private readonly ArchiveContext _context;

        public TrunksController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Trunks
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;

            var allRecords = await _context.Trunks.Include(t => t.CIdNavigation).ToListAsync();

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

        // GET: Trunks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trunk = await _context.Trunks
                .Include(t => t.CIdNavigation)
                .FirstOrDefaultAsync(m => m.TId == id);
            if (trunk == null)
            {
                return NotFound();
            }

            return View(trunk);
        }

        // GET: Trunks/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: Trunks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TId,BuildingNamePlanTitle,PlanYear,ArchitectFirmAssociated,FolderName,Notes,Links,CId")] Trunk trunk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trunk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", trunk.CId);
            return View(trunk);
        }

        // GET: Trunks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trunk = await _context.Trunks.FindAsync(id);
            if (trunk == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", trunk.CId);
            return View(trunk);
        }

        // POST: Trunks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TId,BuildingNamePlanTitle,PlanYear,ArchitectFirmAssociated,FolderName,Notes,Links,CId")] Trunk trunk)
        {
            if (id != trunk.TId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trunk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrunkExists(trunk.TId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", trunk.CId);
            return View(trunk);
        }

        // GET: Trunks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trunk = await _context.Trunks
                .Include(t => t.CIdNavigation)
                .FirstOrDefaultAsync(m => m.TId == id);
            if (trunk == null)
            {
                return NotFound();
            }

            return View(trunk);
        }

        // POST: Trunks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trunk = await _context.Trunks.FindAsync(id);
            if (trunk != null)
            {
                _context.Trunks.Remove(trunk);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrunkExists(int id)
        {
            return _context.Trunks.Any(e => e.TId == id);
        }
    }
}
