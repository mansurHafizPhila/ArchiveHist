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
    public class ResearchesController : Controller
    {
        private readonly ArchiveContext _context;
        private DateOnly? parsedDate;

        public ResearchesController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Researches
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

            // Start with all research
            var query = _context.Researches.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(r =>
                    (r.ResearcherName != null && r.ResearcherName.Contains(searchString)) ||
                    (r.Date != null && r.Date.ToString().Contains(searchString))
                    || (r.Team != null && r.Team.Contains(searchString))
                    || (r.VisitedArchives != null && r.VisitedArchives.Contains(searchString))
                    || (r.TopicCategory != null && r.TopicCategory.Contains(searchString))
                    || (r.SpecificInquiry != null && r.SpecificInquiry.Contains(searchString))
                    || (r.DescriptionOfTasksInvolved != null && r.DescriptionOfTasksInvolved.Contains(searchString))
                    || (r.ScansTaken != null && r.ScansTaken.Contains(searchString))
                    || (r.ReceivingArchivist != null && r.ReceivingArchivist.Contains(searchString))
                    || (r.Notes != null && r.Notes.Contains(searchString))
                    || (r.CId != null && r.CId.ToString().Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "ResearcherName")
                {
                    query = query.Where(r => r.ResearcherName != null);
                }
                else if (column == "Date")
                {
                    query = query.Where(r => r.Date != null);
                }
                else if (column == "Team")
                {
                    query = query.Where(r => r.Team != null);
                }
                else if (column == "VisitedArchives")
                {
                    query = query.Where(r => r.VisitedArchives != null);
                }
                else if (column == "TopicCategory")
                {
                    query = query.Where(r => r.TopicCategory != null);
                }
                else if (column == "SpecificInquiry")
                {
                    query = query.Where(r => r.SpecificInquiry != null);
                }
                else if (column == "DescriptionOfTasksInvolved")
                {
                    query = query.Where(r => r.DescriptionOfTasksInvolved != null);
                }
                else if (column == "ScansTaken")
                {
                    query = query.Where(r => r.ScansTaken != null);
                }
                else if (column == "ReceivingArchivist")
                {
                    query = query.Where(r => r.ReceivingArchivist != null);
                }
                else if (column == "Notes")
                {
                    query = query.Where(r => r.Notes != null);
                }
                else if (column == "CId")
                {
                    query = query.Where(r => r.CId != null);
                }
            }
            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                if (column == "ResearcherName")
                {
                    query = query.Where(r => r.ResearcherName == specificData);
                }
                else if (column == "Team")
                {
                    query = query.Where(r => r.Team != null && r.Team.Contains(specificData));
                }
                else if (column == "VisitedArchives")
                {
                    query = query.Where(r => r.VisitedArchives == specificData);
                }
                else if (column == "Date" && DateOnly.TryParse(specificData, out DateOnly itemValue))
                {
                    query = query.Where(r => r.Date == parsedDate);
                }
                else if (column == "TopicCategory")
                {
                    query = query.Where(r => r.TopicCategory != null && r.TopicCategory.Contains(specificData));
                }
                else if (column == "SpecificInquiry")
                {
                    query = query.Where(r => r.SpecificInquiry == specificData);
                }
                else if (column == "DescriptionOfTasksInvolved")
                {
                    query = query.Where(r => r.DescriptionOfTasksInvolved != null && r.DescriptionOfTasksInvolved.Contains(specificData));
                }
                else if (column == "ScansTaken")
                {
                    query = query.Where(r => r.ScansTaken == specificData);
                }
                else if (column == "ReceivingArchivist")
                {
                    query = query.Where(r => r.ReceivingArchivist != null && r.ReceivingArchivist.Contains(specificData));
                }
                else if (column == "Notes")
                {
                    query = query.Where(r => r.Notes == specificData);
                }
                // Fix for CS0136: Renaming the inner 'itemValue' variable to avoid conflict with the outer scope
                else if (column == "CId" && int.TryParse(specificData, out int specificItemValue))
                {
                    query = query.Where(r => r.CId == specificItemValue);
                }
            }
            // Set up ViewBag.Columns for the dropdown
            ViewBag.Columns = new List<string> {
                "All",
                "ResearcherName",
                "Date",
                "Team",
                "VisitedArchives",
                "TopicCategory",
                "SpecificInquiry",
                "DescriptionOfTasksInvolved",
                "ScansTaken",
                "ReceivingArchivist",
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
            var researches = await query.ToListAsync();

            // Example of specific data options 
            var specificDataOptions = new List<string> { "All" };

            // Determine which column to use for specific data options
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "ResearcherName")
                {
                    var options = await _context.Researches
                        .Where(r => r.ResearcherName != null)
                        .Select(r => r.ResearcherName!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Team")
                {
                    var options = await _context.Researches
                        .Where(r => r.Team != null && r.Team.Length > 0)
                        .Select(r => r.Team!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "VisitedArchives")
                {
                    var options = await _context.Researches
                        .Where(r => r.VisitedArchives != null)
                        .Select(r => r.VisitedArchives!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "TopicCategory")
                {
                    var options = await _context.Researches
                        .Where(r => r.TopicCategory != null)
                        .Select(r => r.TopicCategory!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "SpecificInquiry")
                {
                    var options = await _context.Researches
                        .Where(r => r.SpecificInquiry != null)
                        .Select(r => r.SpecificInquiry!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "DescriptionOfTasksInvolved")
                {
                    var options = await _context.Researches
                        .Where(r => r.DescriptionOfTasksInvolved != null)
                        .Select(r => r.DescriptionOfTasksInvolved!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ScansTaken")
                {
                    var options = await _context.Researches
                        .Where(r => r.ScansTaken != null)
                        .Select(r => r.ScansTaken!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "ReceivingArchivist")
                {
                    var options = await _context.Researches
                        .Where(r => r.ReceivingArchivist != null)
                        .Select(r => r.ReceivingArchivist!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Date")
                {
                    var options = await _context.Researches
                        .Where(r => r.Date != null)
                        .Select(r => r.Date.ToString()!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "Notes")
                {
                    var options = await _context.Researches
                        .Where(r => r.Notes != null)
                        .Select(r => r.Notes!)
                        .Distinct()
                        .Take(50)
                        .ToListAsync();

                    specificDataOptions.AddRange(options);
                }
                else if (column == "CId")
                {
                    var data = await _context.Researches
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
                var options = await _context.Researches
                    .Where(r => r.ResearcherName != null)
                    .Select(r => r.ResearcherName!)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                specificDataOptions.AddRange(options);
            }

            ViewBag.SpecificDataOptions = specificDataOptions;

            // Return the view with the list of researches
            return View(researches);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Researches" || category == "All")
                {
                    if (column == "ResearcherName" || column == "All")
                    {
                        var data = await _context.Researches
                            .Where(r => r.ResearcherName != null)
                            .Select(r => r.ResearcherName!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Team")
                    {
                        var data = await _context.Researches
                            .Where(r => r.Team != null && r.Team.Length > 0)
                            .Select(r => r.Team!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "VisitedArchives")
                    {
                        var data = await _context.Researches
                            .Where(r => r.VisitedArchives != null)
                            .Select(r => r.VisitedArchives!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Date")
                    {
                        var data = await _context.Researches
                            .Where(r => r.Date != null)
                            .Select(r => r.Date.ToString()!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync(); ;

                        options.AddRange((IEnumerable<string>)data);
                    }
                    else if (column == "ScansTaken")
                    {
                        var data = await _context.Researches
                            .Where(r => r.ScansTaken != null && r.ScansTaken.Length > 0)
                            .Select(r => r.ScansTaken!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "ReceivingArchivist")
                    {
                        var data = await _context.Researches
                            .Where(r => r.ReceivingArchivist != null)
                            .Select(r => r.ReceivingArchivist!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "TopicCategory")
                    {
                        var data = await _context.Researches
                            .Where(r => r.TopicCategory != null && r.TopicCategory.Length > 0)
                            .Select(r => r.TopicCategory!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "SpecificInquiry")
                    {
                        var data = await _context.Researches
                            .Where(r => r.SpecificInquiry != null)
                            .Select(r => r.SpecificInquiry!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "DescriptionOfTasksInvolved")
                    {
                        var data = await _context.Researches
                            .Where(r => r.DescriptionOfTasksInvolved != null && r.DescriptionOfTasksInvolved.Length > 0)
                            .Select(r => r.DescriptionOfTasksInvolved!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "Notes")
                    {
                        var data = await _context.Photos
                            .Where(r => r.Notes != null)
                            .Select(r => r.Notes!)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(data);
                    }
                    else if (column == "CId")
                    {
                        var data = await _context.Researches
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

        // GET: Researches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var research = await _context.Researches
                .Include(r => r.CIdNavigation)
                .FirstOrDefaultAsync(m => m.RId == id);
            if (research == null)
            {
                return NotFound();
            }

            return View(research);
        }

        // GET: Researches/Create
        public IActionResult Create()
        {
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", 7);
            var research = new Research { CId = 7 };
            return View(research);
        }

        // POST: Researches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RId,ResearcherName,Date,Team,VisitedArchives,TopicCategory,SpecificInquiry,DescriptionOfTasksInvolved,ScansTaken,ReceivingArchivist,Notes,CId")] Research research)
        {
            if (ModelState.IsValid)
            {
                _context.Add(research);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", research.CId);
            return View(research);
        }

        // GET: Researches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var research = await _context.Researches.FindAsync(id);
            if (research == null)
            {
                return NotFound();
            }
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", research.CId);
            return View(research);
        }

        // POST: Researches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RId,ResearcherName,Date,Team,VisitedArchives,TopicCategory,SpecificInquiry,DescriptionOfTasksInvolved,ScansTaken,ReceivingArchivist,Notes,CId")] Research research)
        {
            if (id != research.RId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(research);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResearchExists(research.RId))
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
            ViewData["CId"] = new SelectList(_context.Collections, "CId", "CId", research.CId);
            return View(research);
        }

        // GET: Researches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var research = await _context.Researches
                .Include(r => r.CIdNavigation)
                .FirstOrDefaultAsync(m => m.RId == id);
            if (research == null)
            {
                return NotFound();
            }

            return View(research);
        }

        // POST: Researches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var research = await _context.Researches.FindAsync(id);
            if (research != null)
            {
                _context.Researches.Remove(research);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResearchExists(int id)
        {
            return _context.Researches.Any(e => e.RId == id);
        }
    }
}
