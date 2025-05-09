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
    public class AudioFilesController : Controller
    {
        private readonly ArchiveHistContext _context;

        public AudioFilesController(ArchiveHistContext context)
        {
            _context = context;
        }

        // GET: AudioFiles
        public async Task<IActionResult> Index()
        {
            var adventureContext = _context.AudioFiles.Include(a => a.CIdNavigation);
            return View(await adventureContext.ToListAsync());
        }

        // GET: AudioFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AudioFiles == null)
            {
                return NotFound();
            }

            var audioFile = await _context.AudioFiles
                .Include(a => a.CIdNavigation)
                .FirstOrDefaultAsync(m => m.AId == id);
            if (audioFile == null)
            {
                return NotFound();
            }

            return View(audioFile);
        }

        // GET: AudioFiles/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId");
            return View();
        }

        // POST: AudioFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AId,LinkName,CId")] AudioFile audioFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(audioFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", audioFile.CId);
            return View(audioFile);
        }

        // GET: AudioFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AudioFiles == null)
            {
                return NotFound();
            }

            var audioFile = await _context.AudioFiles.FindAsync(id);
            if (audioFile == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", audioFile.CId);
            return View(audioFile);
        }

        // POST: AudioFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AId,LinkName,CId")] AudioFile audioFile)
        {
            if (id != audioFile.AId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(audioFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AudioFileExists(audioFile.AId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", audioFile.CId);
            return View(audioFile);
        }

        // GET: AudioFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AudioFiles == null)
            {
                return NotFound();
            }

            var audioFile = await _context.AudioFiles
                .Include(a => a.CIdNavigation)
                .FirstOrDefaultAsync(m => m.AId == id);
            if (audioFile == null)
            {
                return NotFound();
            }

            return View(audioFile);
        }

        // POST: AudioFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AudioFiles == null)
            {
                return Problem("Entity set 'AdventureContext.AudioFiles'  is null.");
            }
            var audioFile = await _context.AudioFiles.FindAsync(id);
            if (audioFile != null)
            {
                _context.AudioFiles.Remove(audioFile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AudioFileExists(int id)
        {
          return (_context.AudioFiles?.Any(e => e.AId == id)).GetValueOrDefault();
        }
    }
}
