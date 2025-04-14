using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Data;
using Microsoft.Extensions.Options;

namespace ArchiveHist.Controllers
{
    public class OversizedsController : Controller
    {
        private readonly ArchiveContext _context;

        public OversizedsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Oversizeds
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

            // Start with all oversizeds
            var query = _context.Oversizeds.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(o =>
                    (o.BuildingName != null && o.BuildingName.Contains(searchString)) ||
                    (o.YearRange != null && o.YearRange.ToString().Contains(searchString))
                    || (o.CompanyArchitect != null && o.CompanyArchitect.Contains(searchString))
                    || (o.Drawer != null && o.Drawer.Contains(searchString))
                    || (o.SideNotes != null && o.SideNotes.Contains(searchString))
                    || (o.CId != null && o.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "BuildingName")
                {
                    query = query.Where(o => o.BuildingName != null);
                }
                else if (column == "YearRange")
                {
                    query = query.Where(o => o.YearRange != null);
                }
                else if (column == "CompanyArchitect")
                {
                    query = query.Where(o => o.CompanyArchitect != null);
                }
                else if (column == "Drawer")
                {
                    query = query.Where(o => o.Drawer != null);
                }
                else if (column == "SideNotes")
                {
                    query = query.Where(o => o.SideNotes != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(o => o.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "BuildingName")
                {
                    query = query.Where(o => o.BuildingName == specificData);
                }
                else if (column == "CompanyArchitect")
                {
                    query = query.Where(o => o.CompanyArchitect != null && o.CompanyArchitect.Contains(specificData));
                }
                else if (column == "Drawer")
                {
                    query = query.Where(o => o.Drawer == specificData);
                }
                else if (column == "YearRange" && int.TryParse(specificData, out int itemValue))
                {
                    query = query.Where(o => o.YearRange == specificData);
                }
                else if (column == "SideNotes")
                {
                    query = query.Where(o => o.SideNotes == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(o => o.CId == specificItemValue);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "BuildingName",
                "CompanyArchitect",
                "Drawer",
                "YearRange",
                "SideNotes",
                "CId"
            };
            // Get the total count before pagination
            var totalCount = await query.CountAsync();
            ViewBag.TotalCount = totalCount;

            //var allRecords = await _context.Oversizeds.Include(o => o.CIdNavigation).ToListAsync();

            //ViewBag.TotalCount = allRecords.Count;

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
            var oversizeds = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "BuildingName")
                {
                    var options = await _context.Oversizeds
                        .Where(o => o.BuildingName != null)
                        .Select(o => o.BuildingName!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CompanyArchitect")
                {
                    var options = await _context.Oversizeds
                        .Where(o => o.CompanyArchitect != null && o.CompanyArchitect.Length > 0)
                        .Select(o => o.CompanyArchitect!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Drawer")
                {
                    var options = await _context.Oversizeds
                        .Where(o => o.Drawer != null)
                        .Select(o => o.Drawer!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "YearRange")
                {
                    var options = await _context.Oversizeds
                        .Where(o => o.YearRange != null)
                        .Select(o => o.YearRange.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "SideNotes")
                {
                    var options = await _context.Oversizeds
                        .Where(o => o.SideNotes != null)
                        .Select(o => o.SideNotes!)
                        .Distinct()
                        .Take(50)
                    .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Oversizeds
                        .Where(o => o.CId != null)
                        .Select(o => o.CId.ToString())
                        .Distinct()
                        .Take(50)
                    .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to BuildingName when "All" is selected
                var options = await _context.Oversizeds
                    .Where(o => o.BuildingName != null)
                    .Select(o => o.BuildingName!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of oversizeds
            return View(oversizeds);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Oversizeds" || category == "All")
                {
                    if (column == "BuildingName" || column == "All")
                    {
                        var data = await _context.Oversizeds
                            .Where(o => o.BuildingName != null)
                            .Select(o => o.BuildingName!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CompanyArchitect")
                    {
                        var data = await _context.Oversizeds
                            .Where(o => o.CompanyArchitect != null && o.CompanyArchitect.Length > 0)
                            .Select(o => o.CompanyArchitect!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Drawer")
                    {
                        var data = await _context.Oversizeds
                            .Where(o => o.Drawer != null)
                            .Select(o => o.Drawer!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "YearRange")
                    {
                        var data = await _context.Oversizeds
                            .Where(o => o.YearRange != null)
                            .Select(o => o.YearRange.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "SideNotes")
                    {
                        var data = await _context.Oversizeds
                            .Where(o => o.SideNotes != null)
                            .Select(o => o.SideNotes!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Oversizeds
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


        // GET: Oversizeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds
                .Include(o => o.CIdNavigation)
                .FirstOrDefaultAsync(m => m.OId == id);
            if (oversized == null)
            {
                return NotFound();
            }

            return View(oversized);
        }

        // GET: Oversizeds/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 3);
            var oversized = new Oversized { CId = 3 };
            return View(oversized);
        }

        // POST: Oversizeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OId,BuildingName,YearRange,CompanyArchitect,Drawer,SideNotes,CId")] Oversized oversized)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oversized);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // GET: Oversizeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds.FindAsync(id);
            if (oversized == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // POST: Oversizeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OId,BuildingName,YearRange,CompanyArchitect,Drawer,SideNotes,CId")] Oversized oversized)
        {
            if (id != oversized.OId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oversized);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OversizedExists(oversized.OId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", oversized.CId);
            return View(oversized);
        }

        // GET: Oversizeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oversized = await _context.Oversizeds
                .Include(o => o.CIdNavigation)
                .FirstOrDefaultAsync(m => m.OId == id);
            if (oversized == null)
            {
                return NotFound();
            }

            return View(oversized);
        }

        // POST: Oversizeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oversized = await _context.Oversizeds.FindAsync(id);
            if (oversized != null)
            {
                _context.Oversizeds.Remove(oversized);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OversizedExists(int id)
        {
            return _context.Oversizeds.Any(e => e.OId == id);
        }
    }
}
