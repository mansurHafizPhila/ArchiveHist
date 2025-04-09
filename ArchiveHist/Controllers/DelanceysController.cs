using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.Packaging;
using Microsoft.Extensions.Options;

namespace ArchiveHist.Controllers
{
    public class DelanceysController : Controller
    {
        private readonly ArchiveContext _context;
        private IEnumerable<string> options;

        public DelanceysController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Delanceys
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber, string searchString, string category, string column, string specificData, string? allRecords)
        {
            int pageSizeValue = pageSize ?? 50; // Default to 50 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            // Set up ViewBag for search and pagination
            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentCategory = category ?? "All";
            ViewBag.CurrentColumn = column ?? "All";
            ViewBag.CurrentSpecificData = specificData ?? "All";
            ViewBag.Title = "Collection of Tables";

            // Start with all delanceys
            var query = _context.Delanceys.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d =>
                    (d.FileCabinetDrawerNumber != null && d.FileCabinetDrawerNumber.Contains(searchString)) ||
                    (d.Description != null && d.Description.Contains(searchString))
                    || (d.Address != null && d.Address.Contains(searchString))
                    || (d.Item != null && d.Item.ToString().Contains(searchString))
                    || (d.Type != null && d.Type.Contains(searchString))
                    || (d.Format != null && d.Format.Contains(searchString))
                    || (d.DateOfCreation != null && d.DateOfCreation.Contains(searchString))
                    || (d.Title != null && d.Title.Contains(searchString))
                    || (d.Creator != null && d.Creator.Contains(searchString))
                    || (d.MakersMarks != null && d.MakersMarks.Contains(searchString))
                    || (d.CId != null && d.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "FileCabinetDrawerNumber")
                {
                    query = query.Where(d => d.FileCabinetDrawerNumber != null);
                }
                else if (column == "Description")
                {
                    query = query.Where(d => d.Description != null);
                }
                else if (column == "Address")
                {
                    query = query.Where(d => d.Address != null);
                }
                else if (column == "Item")
                {
                    query = query.Where(d => d.Item != null);
                }
                else if (column == "Type")
                {
                    query = query.Where(d => d.Type != null);
                }
                else if (column == "Format")
                {
                    query = query.Where(d => d.Format != null);
                }
                else if (column == "DateOfCreation")
                {
                    query = query.Where(d => d.DateOfCreation != null);
                }
                else if (column == "Title")
                {
                    query = query.Where(d => d.Title != null);
                }
                else if (column == "Creator")
                {
                    query = query.Where(d => d.Creator != null);
                }
                else if (column == "MakersMarks")
                {
                    query = query.Where(d => d.MakersMarks != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(d => d.CId != null);
                }
            }

            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "FileCabinetDrawerNumber")
                {
                    query = query.Where(d => d.FileCabinetDrawerNumber == specificData);
                }
                else if (column == "Description")
                {
                    query = query.Where(d => d.Description != null && d.Description.Contains(specificData));
                }
                else if (column == "Address")
                {
                    query = query.Where(d => d.Address == specificData);
                }
                else if (column == "Item" && int.TryParse(specificData, out int itemValue))
                {
                    query = query.Where(d => d.Item == itemValue);
                }
                else if (column == "Type")
                {
                    query = query.Where(d => d.Type == specificData);
                }
                else if (column == "Format")
                {
                    query = query.Where(d => d.Format == specificData);
                }
                else if (column == "DateOfCreation")
                {
                    query = query.Where(d => d.DateOfCreation == specificData);
                }
                else if (column == "Title")
                {
                    query = query.Where(d => d.Title == specificData);
                }
                else if (column == "Creator")
                {
                    query = query.Where(d => d.Creator == specificData);
                }
                else if (column == "MakersMarks")
                {
                    query = query.Where(d => d.MakersMarks == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(d => d.CId == specificItemValue);
                }   
            }

            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "FileCabinetDrawerNumber",
                "Description",
                "Address",
                "Item",
                "Type",
                "Format",
                "DateOfCreation",
                "Title",
                "Creator",
                "MakersMarks",
                "CId"
            };

            // Get the total count before pagination
            var totalCount = await query.CountAsync();
            ViewBag.TotalCount = totalCount;

            // Calculate total pages
            ViewBag.TotalPages = pageSizeValue == -1
                ? 1
                : (int)Math.Ceiling((double)totalCount / pageSizeValue);

            // Apply pagination only if not showing all
            if (pageSizeValue != -1)
            {
                query = query
                    .Skip((pageNumberValue - 1) * pageSizeValue)
                    .Take(pageSizeValue);
            }

            // Load the data
            var delanceys = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "FileCabinetDrawerNumber")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.FileCabinetDrawerNumber != null)
                        .Select(d => d.FileCabinetDrawerNumber!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Description")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Description != null && d.Description.Length > 0)
                        .Select(d => d.Description!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Address")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Address != null)
                        .Select(d => d.Address!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Item")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Item != null)
                        .Select(d => d.Item.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Type")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Type != null)
                        .Select(d => d.Type!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Format")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Format != null)
                        .Select(d => d.Format!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "DateOfCreation")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.DateOfCreation != null)
                        .Select(d => d.DateOfCreation!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Title")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Title != null)
                        .Select(d => d.Title!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Creator")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.Creator != null)
                        .Select(d => d.Creator!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "MakersMarks")
                {
                    var options = await _context.Delanceys
                        .Where(d => d.MakersMarks != null)
                        .Select(d => d.MakersMarks!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Delanceys
                        .Where(d => d.CId != null)
                        .Select(d => d.CId.ToString())
                        .Distinct()
                        .Take(50)
                    .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to FileCabinetDrawerNumber when "All" is selected
                var options = await _context.Delanceys
                    .Where(d => d.FileCabinetDrawerNumber != null)
                    .Select(d => d.FileCabinetDrawerNumber!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of delanceys
            return View(delanceys);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Delanceys" || category == "All")
                {
                    if (column == "FileCabinetDrawerNumber" || column == "All")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.FileCabinetDrawerNumber != null)
                            .Select(d => d.FileCabinetDrawerNumber!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Description")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Description != null && d.Description.Length > 0)
                            .Select(d => d.Description!)
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Address")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Address != null)
                            .Select(d => d.Address!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Item")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Item != null)
                            .Select(d => d.Item!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Type")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Type != null)
                            .Select(d => d.Type!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Format")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Format != null)
                            .Select(d => d.Format!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "DateOfCreation")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.DateOfCreation != null)
                            .Select(d => d.DateOfCreation!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Title")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Title != null)
                            .Select(d => d.Title!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Creator")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.Creator != null)
                            .Select(d => d.Creator!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "MakersMarks")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.MakersMarks != null)
                            .Select(d => d.MakersMarks!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Delanceys
                            .Where(d => d.CId != null)
                            .Select(d => d.CId!.ToString())
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 1);
            var delancey = new Delancey { CId = 1 };
            return View(delancey);
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
