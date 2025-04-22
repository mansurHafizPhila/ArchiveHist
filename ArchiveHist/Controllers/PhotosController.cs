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
    public class PhotosController : Controller
    {
        private readonly ArchiveContext _context;

        public PhotosController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Photos
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

            // Start with all photos
            var query = _context.Photos.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    (p.Title != null && p.Title.Contains(searchString)) ||
                    (p.Year != null && p.Year.ToString().Contains(searchString))
                    || (p.ArtistAgency != null && p.ArtistAgency.Contains(searchString))
                    || (p.Link != null && p.Link.Contains(searchString))
                    || (p.Notes != null && p.Notes.Contains(searchString))
                    || (p.CId != null && p.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    query = query.Where(p => p.Title != null);
                }
                else if (column == "Year")
                {
                    query = query.Where(p => p.Year != null);
                }
                else if (column == "ArtistAgency")
                {
                    query = query.Where(p => p.ArtistAgency != null);
                }
                else if (column == "Link")
                {
                    query = query.Where(p => p.Link != null);
                }
                else if (column == "Notes")
                {
                    query = query.Where(p => p.Notes != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(p => p.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "Title")
                {
                    query = query.Where(p => p.Title == specificData);
                }
                else if (column == "ArtistAgency")
                {
                    query = query.Where(p => p.ArtistAgency != null && p.ArtistAgency.Contains(specificData));
                }
                else if (column == "Link")
                {
                    query = query.Where(p => p.Link == specificData);
                }
                else if (column == "Year" && int.TryParse(specificData, out int itemValue))
                {
                    query = query.Where(p => p.Year == specificData);
                }
                else if (column == "Notes")
                {
                    query = query.Where(p => p.Notes == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(p => p.CId == specificItemValue);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "Title",
                "ArtistAgency",
                "Link",
                "Year",
                "Notes",
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
            if (pageSizeValue != -1)
            {
                query = query
                    .Skip((pageNumberValue - 1) * pageSizeValue)
                    .Take(pageSizeValue);
            }

            // Load the data
            var photos = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    var options = await _context.Photos
                        .Where(p => p.Title != null)
                        .Select(p => p.Title!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ArtistAgency")
                {
                    var options = await _context.Photos
                        .Where(p => p.ArtistAgency != null && p.ArtistAgency.Length > 0)
                        .Select(p => p.ArtistAgency!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Link")
                {
                    var options = await _context.Photos
                        .Where(p => p.Link != null)
                        .Select(p => p.Link!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Year")
                {
                    var options = await _context.Photos
                        .Where(p => p.Year != null)
                        .Select(p => p.Year.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Notes")
                {
                    var options = await _context.Photos
                        .Where(p => p.Notes != null)
                        .Select(p => p.Notes!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Photos
                        .Where(p => p.CId != null)
                        .Select(p => p.CId.ToString())
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to Title when "All" is selected
                var options = await _context.Photos
                    .Where(p => p.Title != null)
                    .Select(p => p.Title!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of photos
            return View(photos);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Photos" || category == "All")
                {
                    if (column == "Title" || column == "All")
                    {
                        var data = await _context.Photos
                            .Where(p => p.Title != null)
                            .Select(p => p.Title!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "ArtistAgency")
                    {
                        var data = await _context.Photos
                            .Where(p => p.ArtistAgency != null && p.ArtistAgency.Length > 0)
                            .Select(p => p.ArtistAgency!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Link")
                    {
                        var data = await _context.Photos
                            .Where(p => p.Link != null)
                            .Select(p => p.Link!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Year")
                    {
                        var data = await _context.Photos
                            .Where(p => p.Year != null)
                            .Select(p => p.Year.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Notes")
                    {
                        var data = await _context.Photos
                            .Where(p => p.Notes != null)
                            .Select(p => p.Notes!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Photos
                            .Where(p => p.CId != null)
                            .Select(p => p.CId.ToString())
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

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photos/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 4);
            var photo = new Photo { CId = 4 };
            return View(photo);
        }

        // POST: Photos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PId,Title,ArtistAgency,Year,Link,Notes,CId")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", photo.CId);
            return View(photo);
        }

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", photo.CId);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PId,Title,ArtistAgency,Year,Link,Notes,CId")] Photo photo)
        {
            if (id != photo.PId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.PId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", photo.CId);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.PId == id);
        }
    }
}
