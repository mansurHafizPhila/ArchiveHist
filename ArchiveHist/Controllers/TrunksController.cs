using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArchiveHist.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArchiveHist.Controllers
{
    public class TrunksController : Controller
    {
        private readonly ArchiveContext _context;
        private DateOnly? parsedDate;

        public TrunksController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Trunks
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

            // Start with all trunks
            var query = _context.Trunks.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t =>
                    (t.BuildingNamePlanTitle != null && t.BuildingNamePlanTitle.Contains(searchString)) ||
                    (t.PlanYear != null && t.PlanYear.ToString().Contains(searchString))
                    || (t.ArchitectFirmAssociated != null && t.ArchitectFirmAssociated.Contains(searchString))
                    || (t.FolderName != null && t.FolderName.Contains(searchString))
                    || (t.Links != null && t.Links.Contains(searchString))
                    || (t.Notes != null && t.Notes.Contains(searchString))
                    || (t.CId != null && t.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "BuildingNamePlanTitle")
                {
                    query = query.Where(t => t.BuildingNamePlanTitle != null);
                }
                else if (column == "PlanYear")
                {
                    query = query.Where(t => t.PlanYear != null);
                }
                else if (column == "ArchitectFirmAssociated")
                {
                    query = query.Where(t => t.ArchitectFirmAssociated != null);
                }
                else if (column == "Links")
                {
                    query = query.Where(t => t.Links != null);
                }
                else if (column == "FolderName")
                {
                    query = query.Where(t => t.FolderName != null);
                }
                else if (column == "Notes")
                {
                    query = query.Where(t => t.Notes != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(t => t.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "BuildingNamePlanTitle")
                {
                    query = query.Where(t => t.BuildingNamePlanTitle == specificData);
                }
                else if (column == "ArchitectFirmAssociated")
                {
                    query = query.Where(t => t.ArchitectFirmAssociated != null && t.ArchitectFirmAssociated.Contains(specificData));
                }
                else if (column == "Links")
                {
                    query = query.Where(t => t.Links == specificData);
                }
                else if (column == "PlanYear" && DateOnly.TryParse(specificData, out DateOnly itemValue))
                {
                    query = query.Where(t => t.PlanYear == parsedDate);
                }
                else if (column == "FolderName")
                {
                    query = query.Where(t => t.FolderName == specificData);
                }
                else if (column == "Notes")
                {
                    query = query.Where(t => t.Notes == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(t => t.CId == specificItemValue);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "BuildingNamePlanTitle",
                "ArchitectFirmAssociated",
                "Links",
                "FolderName",
                "PlanYear",
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
            var trunks = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "BuildingNamePlanTitle")
                {
                    var options = await _context.Trunks
                        .Where(t => t.BuildingNamePlanTitle != null)
                        .Select(t => t.BuildingNamePlanTitle!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ArchitectFirmAssociated")
                {
                    var options = await _context.Trunks
                        .Where(t => t.ArchitectFirmAssociated != null && t.ArchitectFirmAssociated.Length > 0)
                        .Select(t => t.ArchitectFirmAssociated!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "FolderName")
                {
                    var options = await _context.Trunks
                        .Where(t => t.FolderName != null)
                        .Select(t => t.FolderName!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Links")
                {
                    var options = await _context.Trunks
                        .Where(t => t.Links != null)
                        .Select(t => t.Links!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "PlanYear")
                {
                    var options = await _context.Trunks
                        .Where(t => t.PlanYear != null)
                        .Select(t => t.PlanYear.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Notes")
                {
                    var options = await _context.Trunks
                        .Where(t => t.Notes != null)
                        .Select(t => t.Notes!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Trunks
                        .Where(t => t.CId != null)
                        .Select(t => t.CId.ToString())
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(data);
                }
            }
            else
            {
                // Default to BuildingNamePlanTitle when "All" is selected
                var options = await _context.Trunks
                    .Where(t => t.BuildingNamePlanTitle != null)
                    .Select(t => t.BuildingNamePlanTitle!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of photos
            return View(trunks);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Trunks" || category == "All")
                {
                    if (column == "BuildingNamePlanTitle" || column == "All")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.BuildingNamePlanTitle != null)
                            .Select(t => t.BuildingNamePlanTitle!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "ArchitectFirmAssociated")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.ArchitectFirmAssociated != null && t.ArchitectFirmAssociated.Length > 0)
                            .Select(t => t.ArchitectFirmAssociated!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "FolderName")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.FolderName != null)
                            .Select(t => t.FolderName!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();
                        options.AddRange(data);
                    }
                    else if (column == "Links")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.Links != null)
                            .Select(t => t.Links!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "PlanYear")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.PlanYear != null)
                            .Select(t => t.PlanYear.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "Notes")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.Notes != null)
                            .Select(t => t.Notes!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Trunks
                            .Where(t => t.CId != null)
                            .Select(t => t.CId.ToString())
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

        // GET: Trunks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 8);
            var trunk = new Trunk { CId = 8 };
            return View(trunk);
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
            if (id == null)
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
            if (id == null)
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
            return _context.Trunks.Any(e => e.TId == id);
        }
    }
}
