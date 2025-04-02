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
    public class DelanceysController : Controller
    {
        private readonly ArchiveContext _context;

        public DelanceysController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Delanceys
        public async Task<IActionResult> Index()
        {
            var archiveContext = _context.Delanceys.Include(d => d.CIdNavigation);
            return View(await archiveContext.ToListAsync());
        }

        // GET: Delanceys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delancey = await _context.Delanceys
                .Include(d => d.CIdNavigation)
                .FirstOrDefaultAsync(m => m.DId == id);
            if (delancey == null)
            {
                return NotFound();
            }

            return View(delancey);
        }

        // GET: Delanceys/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: Delanceys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DId,FileCabinetDrawerNumber,Address,Item,Type,Format,DateOfCreation,Title,Creator,Description,MakersMarks,CId")] Delancey delancey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(delancey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", delancey.CId);
            return View(delancey);
        }

        // GET: Delanceys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delancey = await _context.Delanceys.FindAsync(id);
            if (delancey == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", delancey.CId);
            return View(delancey);
        }

        // POST: Delanceys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DId,FileCabinetDrawerNumber,Address,Item,Type,Format,DateOfCreation,Title,Creator,Description,MakersMarks,CId")] Delancey delancey)
        {
            if (id != delancey.DId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delancey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DelanceyExists(delancey.DId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", delancey.CId);
            return View(delancey);
        }

        // GET: Delanceys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delancey = await _context.Delanceys
                .Include(d => d.CIdNavigation)
                .FirstOrDefaultAsync(m => m.DId == id);
            if (delancey == null)
            {
                return NotFound();
            }

            return View(delancey);
        }

        // POST: Delanceys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delancey = await _context.Delanceys.FindAsync(id);
            if (delancey != null)
            {
                _context.Delanceys.Remove(delancey);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DelanceyExists(int id)
        {
            return _context.Delanceys.Any(e => e.DId == id);
        }
    }
}
