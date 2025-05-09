﻿using System;
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
        private readonly ArchiveHistContext _context;

        public TrunksController(ArchiveHistContext context)
        {
            _context = context;
        }

        // GET: Trunks
        public async Task<IActionResult> Index()
        {
            var archiveHistContext = _context.Trunks.Include(t => t.CIdNavigation);
            return View(await archiveHistContext.ToListAsync());
        }

        // GET: Trunks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Trunks == null)
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
            if (id == null || _context.Trunks == null)
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
            if (id == null || _context.Trunks == null)
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
            if (_context.Trunks == null)
            {
                return Problem("Entity set 'ArchiveHistContext.Trunks'  is null.");
            }
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
          return (_context.Trunks?.Any(e => e.TId == id)).GetValueOrDefault();
        }
    }
}
