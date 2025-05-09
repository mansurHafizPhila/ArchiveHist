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
    public class MapsController : Controller
    {
        private readonly ArchiveHistContext _context;

        public MapsController(ArchiveHistContext context)
        {
            _context = context;
        }

        // GET: Maps
        public async Task<IActionResult> Index()
        {
            var archiveHistContext = _context.Maps.Include(m => m.CIdNavigation);
            return View(await archiveHistContext.ToListAsync());
        }

        // GET: Maps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Maps == null)
            {
                return NotFound();
            }

            var map = await _context.Maps
                .Include(m => m.CIdNavigation)
                .FirstOrDefaultAsync(m => m.MId == id);
            if (map == null)
            {
                return NotFound();
            }

            return View(map);
        }

        // GET: Maps/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: Maps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MId,MapName,YearRange,ArtistManufacturer,DigitizedLink,Removed,CId")] Map map)
        {
            if (ModelState.IsValid)
            {
                _context.Add(map);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", map.CId);
            return View(map);
        }

        // GET: Maps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Maps == null)
            {
                return NotFound();
            }

            var map = await _context.Maps.FindAsync(id);
            if (map == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", map.CId);
            return View(map);
        }

        // POST: Maps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MId,MapName,YearRange,ArtistManufacturer,DigitizedLink,Removed,CId")] Map map)
        {
            if (id != map.MId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(map);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MapExists(map.MId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", map.CId);
            return View(map);
        }

        // GET: Maps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Maps == null)
            {
                return NotFound();
            }

            var map = await _context.Maps
                .Include(m => m.CIdNavigation)
                .FirstOrDefaultAsync(m => m.MId == id);
            if (map == null)
            {
                return NotFound();
            }

            return View(map);
        }

        // POST: Maps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Maps == null)
            {
                return Problem("Entity set 'ArchiveHistContext.Maps'  is null.");
            }
            var map = await _context.Maps.FindAsync(id);
            if (map != null)
            {
                _context.Maps.Remove(map);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MapExists(int id)
        {
          return (_context.Maps?.Any(e => e.MId == id)).GetValueOrDefault();
        }
    }
}
