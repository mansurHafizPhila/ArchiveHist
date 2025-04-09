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
    public class CollectionsController : Controller
    {
        private readonly ArchiveContext _context;

        public CollectionsController(ArchiveContext context)
        {
            _context = context;
        }

        // GET: Collections
        public async Task<IActionResult> Index(int? pageSize, int? pageNumber, string searchString, string category, string column, string specificData)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            // Set up ViewBag for search and pagination
            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentCategory = category ?? "All";
            ViewBag.CurrentColumn = column ?? "All";
            ViewBag.CurrentSpecificData = specificData ?? "All";
            ViewBag.Title = "Collection of Tables";

            // Start with all collections
            var query = _context.Collections.AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c =>
                    (c.CName != null && c.CName.Contains(searchString)) ||
                    (c.Description != null && c.Description.Contains(searchString))
                );
            }

            // Apply column filter
            if (!string.IsNullOrEmpty(column) && column != "All")
            {
                if (column == "CName")
                {
                    query = query.Where(c => c.CName != null);
                }
                else if (column == "Description")
                {
                    query = query.Where(c => c.Description != null);
                }
                // Add more column filters as needed
            }

            // Apply specific data filter
            if (!string.IsNullOrEmpty(specificData) && specificData != "All")
            {
                // Implementation depends on your specific needs
                // Example: filter by specific content in the description
                query = query.Where(c =>
                    c.Description != null &&
                    c.Description.Contains(specificData)
                );
            }

            // Get the total count before pagination
            var allRecords = await query.ToListAsync();
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
            ViewBag.Columns = new List<string> { "All", "CName", "Description" };

            // Example of specific data options
            var specificDataOptions = new List<string> { "All" };

            // Add collection names as specific data options
            var collectionNames = await _context.Collections
                .Where(c => c.CName != null)
                .Select(c => c.CName)
                .Distinct()
                .ToListAsync();

            specificDataOptions.AddRange(collectionNames.Where(n => n != null));
            ViewBag.SpecificDataOptions = specificDataOptions;

            return View(allRecords);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string category, string column)
        {
            List<string> options = new List<string> { "All" };

            try
            {
                if (category == "Collections" || category == "All")
                {
                    if (column == "CName" || column == "All")
                    {
                        var names = await _context.Collections
                            .Where(c => c.CName != null)
                            .Select(c => c.CName)
                            .Distinct()
                            .Take(50)
                            .ToListAsync();

                        options.AddRange(names.Where(n => n != null));
                    }
                    else if (column == "Description")
                    {
                        // For descriptions, we might want to provide some meaningful excerpts
                        // This is just an example - you may want to customize this
                        var descriptions = await _context.Collections
                            .Where(c => c.Description != null)
                            .Select(c => c.Description.Length > 30
                                ? c.Description.Substring(0, 27) + "..."
                                : c.Description)
                            .Distinct()
                            .Take(20)
                            .ToListAsync();

                        options.AddRange(descriptions.Where(d => d != null));
                    }
                }
                // Add other categories if needed
            }
            catch (Exception)
            {
                // Just return the default "All" option in case of an error
            }

            return Json(options);
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.CId == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CId,CName,Description")] Collection collection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CId,CName,Description")] Collection collection)
        {
            if (id != collection.CId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.CId))
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
            return View(collection);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.CId == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.CId == id);
        }
    }
}
