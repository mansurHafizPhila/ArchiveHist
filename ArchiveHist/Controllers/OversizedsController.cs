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
    public class OversizedsController : Controller
    {
        private readonly ArchiveContext _context;

        public OversizedsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Oversizeds
        public async Task<IActionResult> Index()
        {
            var archiveContext = _context.Oversizeds.Include(o => o.CIdNavigation);
            return View(await archiveContext.ToListAsync());
        }

        // GET: Oversizeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds
                .Include(o => o.CIdNavigation)
                .FirstOrDefaultAsync(m => m.OId == id);
            if (oversized == null)
            {
                return NotFound();
            }

            return View(oversized);
        }

        // GET: Oversizeds/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: Oversizeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OId,BuildingName,YearRange,CompanyArchitect,Drawer,SideNotes,CId")] Oversized oversized)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oversized);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // GET: Oversizeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds.FindAsync(id);
            if (oversized == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // POST: Oversizeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OId,BuildingName,YearRange,CompanyArchitect,Drawer,SideNotes,CId")] Oversized oversized)
        {
            if (id != oversized.OId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oversized);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OversizedExists(oversized.OId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // GET: Oversizeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds
                .Include(o => o.CIdNavigation)
                .FirstOrDefaultAsync(m => m.OId == id);
            if (oversized == null)
            {
                return NotFound();
            }

            return View(oversized);
        }

        // POST: Oversizeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oversized = await _context.Oversizeds.FindAsync(id);
            if (oversized != null)
            {
                _context.Oversizeds.Remove(oversized);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OversizedExists(int id)
        {
            return _context.Oversizeds.Any(e => e.OId == id);
        }
    }
}
