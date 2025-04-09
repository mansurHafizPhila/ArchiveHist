using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ArchiveHist.Controllers
{
    public class AudioFilesController : Controller
    {
        private readonly ArchiveContext _context;

        public AudioFilesController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: AudioFiles
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber, string searchString, string category, string column, string specificData)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;
            ViewBag.CurrentSearchString = searchString;
            //ViewBag.CurrentCategory = category ?? "All";
            //ViewBag.CurrentColumn = column ?? "All";
            //ViewBag.CurrentSpecificData = specificData ?? "All";

            // Get the base query
            var query = _context.AudioFiles.Include(a => a.CIdNavigation).AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.LinkName != null && a.LinkName.Contains(searchString));
            }

            // Apply category filter (table name filter)
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                // This would need to be customized based on your specific database schema
                if (category == "AudioFiles")
                {
                    // Already filtered to AudioFiles by default
                }
                else if (category == "Collections")
                {
                    // Filter by Collections somehow
                    query = query.Where(a => a.CIdNavigation != null);
                }
                
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "LinkName")
                {
                    query = query.Where(a => a.LinkName != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(a => a.CId != null);
                }
                // Add more column filters as needed
            }

            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                // Implement specific filters based on your requirements
                // For example, filter by specific collection IDs, etc.
                if (int.TryParse(specificData, out int specificId))
                {
                    query = query.Where(a => a.CId == specificId);
                }
                else
                {
                    // Try to filter by link name pattern
                    query = query.Where(a => a.LinkName != null && a.LinkName.Contains(specificData));
                }
            }

            var allRecords = await query.ToListAsync();

            // Process display names
            foreach (var record in allRecords)
            {
                if (record.LinkName != null)
                {
                    // Extract file name from URL
                    string displayName;
                    if (record.LinkName.Contains("UniqueId="))
                    {
                        // Extract the UniqueId parameter
                        var uniqueId = record.LinkName.Split("UniqueId=")[1].Split("&")[0];
                        displayName = $"Audio File {uniqueId.Substring(0, 8)}";
                    }
                    else
                    {
                        // Use last segment of URL path
                        var segments = record.LinkName.TrimEnd('/').Split('/');
                        displayName = segments[segments.Length - 1].Replace("%20", " ");

                        // Remove query parameters if any
                        if (displayName.Contains('?'))
                        {
                            displayName = displayName.Split('?')[0];
                        }
                    }

                    record.DisplayName = displayName;
                }
                else
                {
                    record.DisplayName = "Audio File";
                }
            }

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

            // Get data for dropdown filters
            ViewBag.Categories = new List<string> { "All", "AudioFiles", "Collections" };
            ViewBag.Columns = new List<string> { "All", "LinkName", "CId" };

            // Get specific data items (example: collection IDs)
            var collectionIds = await _context.Collections.Select(a => a.CId.ToString()).ToListAsync();
            var specificDataOptions = new List<string> { "All" };
            specificDataOptions.AddRange(collectionIds);
            ViewBag.SpecificDataOptions = specificDataOptions;

            return View(allRecords);
        }

        // GET: AudioFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId",9);
            var audioFile = new AudioFile { CId = 9 };
            return View(audioFile);
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
            if (id == null)
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

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "AudioFiles" || category == "All")
                {
                    if (column == "LinkName" || column == "All")
                    {
                        // Get unique link names or patterns
                        var linkNames = await _context.AudioFiles
                            .Where(a => a.LinkName != null)
                            .Select(a => a.LinkName)
                            .Distinct()
                            .Take(50) // Limit to prevent overwhelming the dropdown
                            .ToListAsync();

                        // Add shortened versions for display
                        foreach (var link in linkNames)
                        {
                            if (link != null)
                            {
                                var segments = link.TrimEnd('/').Split('/');
                                var displayName = segments[segments.Length - 1].Replace("%20", " ");

                                // Remove query parameters if any
                                if (displayName.Contains('?'))
                                {
                                    displayName = displayName.Split('?')[0];
                                }

                                if (!string.IsNullOrEmpty(displayName) && displayName.Length > 30)
                                {
                                    displayName = displayName.Substring(0, 27) + "...";
                                }

                                options.Add(displayName);
                            }
                        }
                    }
                    else if (column == "CId")
                    {
                        // Get collection IDs
                        var collectionIds = await _context.AudioFiles
                            .Where(a => a.CId != null)
                            .Select(a => a.CId.ToString())
                            .Distinct()
                            .ToListAsync();

                        options.AddRange(collectionIds);
                    }
                }
                else if (category == "Collections")
                {
                    // Get collection IDs
                    var collectionIds = await _context.Collections
                        .Select(c => c.CId.ToString())
                        .ToListAsync();

                    options.AddRange(collectionIds);
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // Return just the "All" option in case of error
            }

            return Json(options);
        }

        // GET: AudioFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
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
            return _context.AudioFiles.Any(e => e.AId == id);
        }
    }
}
