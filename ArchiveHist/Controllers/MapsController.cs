using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Data;

namespace ArchiveHist.Controllers
{
    public class MapsController : Controller
    {
        private readonly ArchiveContext _context;
        private IEnumerable<string> options;

        public MapsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Maps
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber, string searchString, string category, string column, string specificData, string? allRecords)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;

            // Set up ViewBag for search and pagination
            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentCategory = category ?? "All";
            ViewBag.CurrentColumn = column ?? "All";
            ViewBag.CurrentSpecificData = specificData ?? "All";
            ViewBag.Title = "Collection of Tables";

            // Start with all maps
            var query = _context.Maps.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m =>
                    (m.MapName != null && m.MapName.Contains(searchString)) ||
                    (m.YearRange != null && m.YearRange.ToString().Contains(searchString))
                    || (m.ArtistManufacturer != null && m.ArtistManufacturer.Contains(searchString))
                    || (m.DigitizedLink != null && m.DigitizedLink.Contains(searchString))
                    || (m.Removed != null && m.Removed.Contains(searchString))
                    || (m.CId != null && m.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "MapName")
                {
                    query = query.Where(m => m.MapName != null);
                }
                else if (column == "YearRange")
                {
                    query = query.Where(m => m.YearRange != null);
                }
                else if (column == "ArtistManufacturer")
                {
                    query = query.Where(m => m.ArtistManufacturer != null);
                }
                else if (column == "DigitizedLink")
                {
                    query = query.Where(m => m.DigitizedLink != null);
                }
                else if (column == "Removed")
                {
                    query = query.Where(m => m.Removed != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(m => m.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "MapName")
                {
                    query = query.Where(m => m.MapName == specificData);
                }
                else if (column == "ArtistManufacturer")
                {
                    query = query.Where(m => m.ArtistManufacturer != null && m.ArtistManufacturer.Contains(specificData));
                }
                else if (column == "DigitizedLink")
                {
                    query = query.Where(m => m.DigitizedLink == specificData);
                }
                else if (column == "YearRange" && int.TryParse(specificData, out int itemValue))
                {
                    query = query.Where(m => m.YearRange == specificData);
                }
                else if (column == "Removed")
                {
                    query = query.Where(m => m.Removed == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(m => m.CId == specificItemValue);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "MapName",
                "ArtistManufacturer",
                "DigitizedLink",
                "YearRange",
                "Removed",
                "CId"
            };

            // Get the total count before pagination
            var totalCount = await query.CountAsync();
            ViewBag.TotalCount = totalCount;

            // Calculate total pages
            ViewBag.TotalPages = pageSizeValue == -1
                ? 1
                : (int)Math.Ceiling((double)ViewBag.TotalCount / pageSizeValue);

            // Apply pagination only if not showing all
            {
                query = query
                    .Skip((pageNumberValue - 1) * pageSizeValue)
                    .Take(pageSizeValue);
            }

            // Load the data
            var maps = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "MapName")
                {
                    var options = await _context.Maps
                        .Where(m => m.MapName != null)
                        .Select(m => m.MapName!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ArtistManufacturer")
                {
                    var options = await _context.Maps
                        .Where(m => m.ArtistManufacturer != null && m.ArtistManufacturer.Length > 0)
                        .Select(m => m.ArtistManufacturer!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "DigitizedLink")
                {
                    var options = await _context.Maps
                        .Where(m => m.DigitizedLink != null)
                        .Select(m => m.DigitizedLink!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "YearRange")
                {
                    var options = await _context.Maps
                        .Where(m => m.YearRange != null)
                        .Select(m => m.YearRange.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Maps
                        .Where(m => m.CId != null)
                        .Select(m => m.CId.ToString())
                        .Distinct()
                        .Take(50)
                    .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to MapName when "All" is selected
                var options = await _context.Maps
                    .Where(m => m.MapName != null)
                    .Select(m => m.MapName!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of maps
            return View(maps);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Maps" || category == "All")
                {
                    if (column == "MapName" || column == "All")
                    {
                        var data = await _context.Maps
                            .Where(m => m.MapName != null)
                            .Select(m => m.MapName!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "ArtistManufacturer")
                    {
                        var data = await _context.Maps
                            .Where(m => m.ArtistManufacturer != null && m.ArtistManufacturer.Length > 0)
                            .Select(m => m.ArtistManufacturer!)
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "DigitizedLink")
                    {
                        var data = await _context.Maps
                            .Where(m => m.DigitizedLink != null)
                            .Select(m => m.DigitizedLink!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "YearRange")
                    {
                        var data = await _context.Maps
                            .Where(m => m.YearRange != null)
                            .Select(m => m.YearRange!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Removed")
                    {
                        var data = await _context.Maps
                            .Where(m => m.Removed != null)
                            .Select(m => m.Removed!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Maps
                            .Where(m => m.CId != null)
                            .Select(m => m.CId!.ToString())
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                }
            }
            catch (Exception)
            {
                // Just return the default "All" option in case of an error
            }

            return Json(options);
        }
        // GET: Maps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId",2);
            var maps = new Map { CId = 2 };
            return View(maps);
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
            if (id == null)
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
            if (id == null)
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
            return _context.Maps.Any(e => e.MId == id);
        }
    }
}
