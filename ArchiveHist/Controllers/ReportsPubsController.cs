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
    public class ReportsPubsController : Controller
    {
        private readonly ArchiveContext _context;
        private DateOnly? parsedDate;

        public ReportsPubsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: ReportsPubs
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

            // Start with all reportsPubs
            var query = _context.ReportsPubs.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(r =>
                    (r.Title != null && r.Title.Contains(searchString)) ||
                    (r.TotalInSeriesCopies != null && r.TotalInSeriesCopies.ToString().Contains(searchString))
                    || (r.AgencyAuthorS != null && r.AgencyAuthorS.Contains(searchString))
                    || (r.Tags != null && r.Tags.Contains(searchString))
                    || (r.Location != null && r.Location.Contains(searchString))
                    || (r.Notes != null && r.Notes.Contains(searchString))
                    || (r.Date != null && r.Date.ToString().Contains(searchString))
                    || (r.CId != null && r.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    query = query.Where(r => r.Title != null);
                }
                else if (column == "TotalInSeriesCopies")
                {
                    query = query.Where(r => r.TotalInSeriesCopies != null);
                }
                else if (column == "AgencyAuthorS")
                {
                    query = query.Where(r => r.AgencyAuthorS != null);
                }
                else if (column == "Tags")
                {
                    query = query.Where(r => r.Tags != null);
                }
                else if (column == "Location")
                {
                    query = query.Where(r => r.Location != null);
                }
                else if (column == "Notes")
                {
                    query = query.Where(r => r.Notes != null);
                }
                else if (column == "Date")
                {
                    query = query.Where(r => r.Date != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(r => r.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "Title")
                {
                    query = query.Where(r => r.Title == specificData);
                }
                else if (column == "AgencyAuthorS")
                {
                    query = query.Where(r => r.AgencyAuthorS != null && r.AgencyAuthorS.Contains(specificData));
                }
                else if (column == "Date" && DateOnly.TryParse(specificData, out DateOnly itemValue))
                {
                    query = query.Where(r => r.Date == parsedDate);
                }
                else if (column == "Tags")
                {
                    query = query.Where(r => r.Tags == specificData);
                }
                else if (column == "Location")
                {
                    query = query.Where(r => r.Location == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "TotalInSeriesCopies" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(r => r.TotalInSeriesCopies == specificItemValue);
                }
                else if (column == "CId" && int.TryParse(specificData, out int specificCIdValue))
                {
                    query = query.Where(r => r.CId == specificCIdValue);
                }
                else if (column == "Notes")
                {
                    query = query.Where(r => r.Notes == specificData);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "Title",
                "TotalInSeriesCopies",
                "AgencyAuthorS",
                "Date",
                "Tags",
                "Location",
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
            var reportsPubs = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.Title != null)
                        .Select(r => r.Title!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "AgencyAuthorS")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.AgencyAuthorS != null && r.AgencyAuthorS.Length > 0)
                        .Select(r => r.AgencyAuthorS!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Tags")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.Tags != null)
                        .Select(r => r.Tags!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "TotalInSeriesCopies")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.TotalInSeriesCopies != null)
                        .Select(r => r.TotalInSeriesCopies.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Date")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.Date != null)
                        .Select(r => r.Date.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Location")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.Location != null)
                        .Select(r => r.Location!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Notes")
                {
                    var options = await _context.ReportsPubs
                        .Where(r => r.Notes != null)
                        .Select(r => r.Notes!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.ReportsPubs
                        .Where(r => r.CId != null)
                        .Select(r => r.CId.ToString())
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to Title when "All" is selected
                var options = await _context.ReportsPubs
                    .Where(r => r.Title != null)
                    .Select(r => r.Title!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of reportsPubs
            return View(reportsPubs);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "ReportsPubs" || category == "All")
                {
                    if (column == "Title" || column == "All")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.Title != null)
                            .Select(r => r.Title!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "AgencyAuthorS")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.AgencyAuthorS != null && r.AgencyAuthorS.Length > 0)
                            .Select(r => r.AgencyAuthorS!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Tags")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.Tags != null)
                            .Select(r => r.Tags!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "TotalInSeriesCopies")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.TotalInSeriesCopies != null)
                            .Select(r => r.TotalInSeriesCopies.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Date")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.Date != null)
                            .Select(r => r.Date.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Location")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.Location != null)
                            .Select(r => r.Location!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Notes")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.Notes != null)
                            .Select(r => r.Notes!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.ReportsPubs
                            .Where(r => r.CId != null)
                            .Select(r => r.CId.ToString())
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 6);
            var reports = new ReportsPub { CId = 6 };
            return View(reports);
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
