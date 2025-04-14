using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Drawing.Printing;
using System.Data;

namespace ArchiveHist.Controllers
{
    public class PoisonBooksController : Controller
    {
        private readonly ArchiveContext _context;
        private DateOnly? parsedDate;

        public PoisonBooksController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: PoisonBooks
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

            // Start with all poisonBooks
            var query = _context.PoisonBooks.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
            query = query.Where(p =>
                (p.Title != null && p.Title.Contains(searchString)) ||
                (p.Date != null && p.Date.ToString().Contains(searchString)) || // Added null check for 'Date'
                (p.Location != null && p.Location.Contains(searchString)) ||
                (p.Author != null && p.Author.Contains(searchString)) ||
                (p.Poison.ToString().Contains(searchString)) || // 'Poison' is a bool, safe to call ToString()
                (p.ArsenicWarning.ToString().Contains(searchString)) || // 'ArsenicWarning' is a bool, safe to call ToString()
                (p.CId != null && p.CId.ToString().Contains(searchString)) // Added null check for 'CId'
            );

            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    query = query.Where(p => p.Title != null);
                }
                else if (column == "Date")
                {
                    query = query.Where(p => p.Date != null);
                }
                else if (column == "Location")
                {
                    query = query.Where(p => p.Location != null);
                }
                else if (column == "Author")
                {
                    query = query.Where(p => p.Author != null);
                }
                else if (column == "Poison")
                {
                    query = query.Where(p => p.Poison != null);
                }
                else if (column == "ArsenicWarning")
                {
                    query = query.Where(p => p.ArsenicWarning != null);
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
                else if (column == "Author")
                {
                    query = query.Where(p => p.Author != null && p.Author.Contains(specificData));
                }
                else if (column == "Location")
                {
                    query = query.Where(p => p.Location == specificData);
                }
                else if (column == "Date" && DateOnly.TryParse(specificData, out DateOnly itemValue))
                {
                    query = query.Where(p => p.Date == parsedDate);
                }
                // Fix for CS0019: Operator '==' cannot be applied to operands of type 'bool' and 'string'
                // The issue occurs because the 'Poison' property is of type 'bool', but the comparison is being made with a string.
                // To fix this, we need to parse the string into a boolean value before comparison.

                else if (column == "Poison" && bool.TryParse(specificData, out bool specificBoolValue))
                {
                    query = query.Where(p => p.Poison == specificBoolValue);
                }
                // Fix for CS0019: Operator '==' cannot be applied to operands of type 'bool' and 'string'
                // The issue occurs because the 'ArsenicWarning' property is of type 'bool', but the comparison is being made with a string.
                // To fix this, we need to parse the string into a boolean value before comparison.

                // Fix for CS0136: Renaming the inner 'specificBoolValue' variable to avoid conflict with the outer scope
                else if (column == "Poison" && bool.TryParse(specificData, out bool specificPoisonValue))
                {
                    query = query.Where(p => p.Poison == specificPoisonValue);
                }
                // Fix for CS0136: Renaming the inner 'specificBoolValue' variable to avoid conflict with the outer scope
                else if (column == "ArsenicWarning" && bool.TryParse(specificData, out bool specificArsenicWarningValue))
                {
                    query = query.Where(p => p.ArsenicWarning == specificArsenicWarningValue);
                    // Fix for CS1929 and CS0472: Adjusting the condition to properly handle the 'bool' type and removing unnecessary null checks
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        query = query.Where(p =>
                            (p.Title != null && p.Title.Contains(searchString)) ||
                            (p.Date != null && p.Date.ToString().Contains(searchString)) ||
                            (p.Location != null && p.Location.Contains(searchString)) ||
                            (p.Author != null && p.Author.Contains(searchString)) ||
                            (p.Poison.ToString().Contains(searchString)) || // Convert 'bool' to string for comparison
                            (p.ArsenicWarning.ToString().Contains(searchString)) || // Convert 'bool' to string for comparison
                            (p.CId != null && p.CId.ToString().Contains(searchString))
                        );
                    }
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
                "Location",
                "Author",
                "Poison",
                "Date",
                "ArsenicWarning",
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
            var poisonBooks = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "Title")
                {
                    var options = await _context.PoisonBooks
                        .Where(p => p.Title != null)
                        .Select(p => p.Title!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Location")
                {
                    var options = await _context.PoisonBooks
                        .Where(p => p.Location != null && p.Location.Length > 0)
                        .Select(p => p.Location!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Author")
                {
                    var options = await _context.PoisonBooks
                        .Where(p => p.Author != null)
                        .Select(p => p.Author!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Poison")
                {
                    specificDataOptions.Add("True");
                    specificDataOptions.Add("False");
                }
                else if (column == "Date")
                {
                    var options = await _context.PoisonBooks
                        .Where(p => p.Date != null)
                        .Select(p => p.Date.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ArsenicWarning")
                {
                    specificDataOptions.Add("True");
                    specificDataOptions.Add("False");
                }
                else if (column == "CId")
                {
                    var data = await _context.PoisonBooks
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
                var options = await _context.PoisonBooks
                    .Where(p => p.Title != null)
                    .Select(p => p.Title!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of photos
            return View(poisonBooks);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "PoisonBooks" || category == "All")
                {
                    if (column == "Title" || column == "All")
                    {
                        var data = await _context.PoisonBooks
                            .Where(p => p.Title != null)
                            .Select(p => p.Title!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Location")
                    {
                        var data = await _context.PoisonBooks
                            .Where(p => p.Location != null && p.Location.Length > 0)
                            .Select(p => p.Location!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Author")
                    {
                        var data = await _context.PoisonBooks
                            .Where(p => p.Author != null)
                            .Select(p => p.Author!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Poison")
                    {
                        options.Add("True");
                        options.Add("False");
                    }
                    else if (column == "Date")
                    {
                        var data = await _context.PoisonBooks
                            .Where(p => p.Date != null)
                            .Select(p => p.Date.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "ArsenicWarning")
                    {
                        options.Add("True");
                        options.Add("False");
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.PoisonBooks
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

        // GET: PoisonBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (poisonBook == null)
            {
                return NotFound();
            }

            return View(poisonBook);
        }

        // GET: PoisonBooks/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 5);
            var poison = new PoisonBook { CId = 5 };
            return View(poison);
        }

        // POST: PoisonBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PId,Title,Location,Author,Date,Poison,ArsenicWarning,CId")] PoisonBook poisonBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poisonBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // GET: PoisonBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks.FindAsync(id);
            if (poisonBook == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // POST: PoisonBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PId,Title,Location,Author,Date,Poison,ArsenicWarning,CId")] PoisonBook poisonBook)
        {
            if (id != poisonBook.PId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poisonBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoisonBookExists(poisonBook.PId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", poisonBook.CId);
            return View(poisonBook);
        }

        // GET: PoisonBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poisonBook = await _context.PoisonBooks
                .Include(p => p.CIdNavigation)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (poisonBook == null)
            {
                return NotFound();
            }

            return View(poisonBook);
        }

        // POST: PoisonBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poisonBook = await _context.PoisonBooks.FindAsync(id);
            if (poisonBook != null)
            {
                _context.PoisonBooks.Remove(poisonBook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoisonBookExists(int id)
        {
            return _context.PoisonBooks.Any(e => e.PId == id);
        }
    }
}
