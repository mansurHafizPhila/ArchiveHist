using ArchiveHist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveHist.Controllers
{
    public class SearchController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> Index(string searchString, int? pageSize, int? pageNumber)
        {
            int pageSizeValue = pageSize ?? 20; // Default to 20 items
            int pageNumberValue = pageNumber ?? 1; // Default to page 1

            ViewBag.PageSize = pageSizeValue;
            ViewBag.PageNumber = pageNumberValue;
            ViewBag.CurrentSearchString = searchString;

            if (string.IsNullOrEmpty(searchString))
            {
                // If no search string, return empty result
                ViewBag.TotalCount = 0;
                ViewBag.TotalPages = 0;
                return View(new CrossTable());
            }

            // empty global search result
            var model = new CrossTable();

            // Query each table for matches to the search string
            var audioFilesTask = SearchAudioFiles(searchString);
            var collectionsTask = SearchCollections(searchString);
            var delanceysTask = SearchDelanceys(searchString);
            var mapsTask = SearchMaps(searchString);
            var oversizedsTask = SearchOversizeds(searchString);
            var photosTask = SearchPhotos(searchString);
            var poisonBooksTask = SearchPoisonBooks(searchString);
            var reportsPubsTask = SearchReportsPubs(searchString);
            var researchesTask = SearchResearches(searchString);
            var trunksTask = SearchTrunks(searchString);

            // Execute all searches in parallel
            await Task.WhenAll(audioFilesTask, collectionsTask, delanceysTask,
                               mapsTask, oversizedsTask, photosTask, poisonBooksTask,
                               reportsPubsTask, researchesTask, trunksTask);

            // Assign results to model
            model.AudioFiles = audioFilesTask.Result;
            model.Collections = collectionsTask.Result;
            model.Delanceys = delanceysTask.Result;
            model.Maps = mapsTask.Result;
            model.Oversizeds = oversizedsTask.Result;
            model.Photos = photosTask.Result;
            model.PoisonBooks = poisonBooksTask.Result;
            model.ReportsPubs = reportsPubsTask.Result;
            model.Researches = researchesTask.Result;
            model.Trunks = trunksTask.Result;

            // Calculate total count across all tables
            model.TotalCount = model.AudioFiles.Count + model.Collections.Count +
                               model.Delanceys.Count + model.Maps.Count +
                               model.Oversizeds.Count + model.Photos.Count +
                               model.PoisonBooks.Count + model.ReportsPubs.Count +
                               model.Researches.Count + model.Trunks.Count;

            ViewBag.TotalCount = model.TotalCount;
            ViewBag.TotalPages = pageSizeValue == -1
                ? 1
                : (int)Math.Ceiling((double)ViewBag.TotalCount / pageSizeValue);

            // Apply pagination across all results
            model = ApplyPagination(model, pageNumberValue, pageSizeValue);

            return View(model);
        }

        private async Task<List<AudioFile>> SearchAudioFiles(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.AudioFiles
                .Where(a => a.LinkName != null && a.LinkName.Contains(searchString)) // Fixed parentheses
                .Include(a => a.CIdNavigation) // Ensure Include is applied to the DbSet, not a boolean expression
                .ToListAsync();
        }

        private async Task<List<Collection>> SearchCollections(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Collections
                .Where(c => (c.CName != null && c.CName.Contains(searchString)) ||
                            (c.Description != null && c.Description.Contains(searchString)))
                .ToListAsync();
        }

        private async Task<List<Delancey>> SearchDelanceys(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Delanceys
                .Where(d => (d.FileCabinetDrawerNumber != null && d.FileCabinetDrawerNumber.Contains(searchString)) ||
                            (d.Description != null && d.Description.Contains(searchString)) ||
                            (d.Address != null && d.Address.Contains(searchString)) ||
                            (d.Item != null && d.Item.ToString().Contains(searchString)) ||
                            (d.Type != null && d.Type.Contains(searchString)) ||
                            (d.Format != null && d.Format.Contains(searchString)) ||
                            (d.DateOfCreation != null && d.DateOfCreation.Contains(searchString)) ||
                            (d.Title != null && d.Title.Contains(searchString)) ||
                            (d.Creator != null && d.Creator.Contains(searchString)) ||
                            (d.MakersMarks != null && d.MakersMarks.Contains(searchString)))
                .Include(d => d.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<Map>> SearchMaps(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Maps
                .Where(m => (m.MapName != null && m.MapName.Contains(searchString)) ||
                            (m.YearRange != null && m.YearRange.ToString().Contains(searchString)) ||
                            (m.ArtistManufacturer != null && m.ArtistManufacturer.Contains(searchString)) ||
                            (m.DigitizedLink != null && m.DigitizedLink.Contains(searchString)) ||
                            (m.Removed != null && m.Removed.Contains(searchString)))
                .Include(m => m.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<Oversized>> SearchOversizeds(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Oversizeds
                .Where(o => (o.BuildingName != null && o.BuildingName.Contains(searchString)) ||
                            (o.YearRange != null && o.YearRange.ToString().Contains(searchString)) ||
                            (o.CompanyArchitect != null && o.CompanyArchitect.Contains(searchString)) ||
                            (o.Drawer != null && o.Drawer.Contains(searchString)) ||
                            (o.SideNotes != null && o.SideNotes.Contains(searchString)))
                .Include(o => o.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<Photo>> SearchPhotos(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Photos
                .Where(p => (p.Title != null && p.Title.Contains(searchString)) ||
                            (p.Year != null && p.Year.ToString().Contains(searchString)) ||
                            (p.ArtistAgency != null && p.ArtistAgency.Contains(searchString)) ||
                            (p.Link != null && p.Link.Contains(searchString)) ||
                            (p.Notes != null && p.Notes.Contains(searchString)))
                .Include(p => p.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<PoisonBook>> SearchPoisonBooks(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.PoisonBooks
                .Where(p => (p.Title != null && p.Title.Contains(searchString)) ||
                            (p.Date != null && p.Date.ToString().Contains(searchString)) ||
                            (p.Location != null && p.Location.Contains(searchString)) ||
                            (p.Author != null && p.Author.Contains(searchString)))
                .Include(p => p.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<ReportsPub>> SearchReportsPubs(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.ReportsPubs
                .Where(r => (r.Title != null && r.Title.Contains(searchString)) ||
                            (r.TotalInSeriesCopies != null && r.TotalInSeriesCopies.ToString().Contains(searchString)) ||
                            (r.AgencyAuthorS != null && r.AgencyAuthorS.Contains(searchString)) ||
                            (r.Tags != null && r.Tags.Contains(searchString)) ||
                            (r.Location != null && r.Location.Contains(searchString)) ||
                            (r.Notes != null && r.Notes.Contains(searchString)) ||
                            (r.Date != null && r.Date.ToString().Contains(searchString)))
                .Include(r => r.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<Research>> SearchResearches(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Researches
                .Where(r => (r.ResearcherName != null && r.ResearcherName.Contains(searchString)) ||
                            (r.Date != null && r.Date.ToString().Contains(searchString)) ||
                            (r.Team != null && r.Team.Contains(searchString)) ||
                            (r.VisitedArchives != null && r.VisitedArchives.Contains(searchString)) ||
                            (r.TopicCategory != null && r.TopicCategory.Contains(searchString)) ||
                            (r.SpecificInquiry != null && r.SpecificInquiry.Contains(searchString)) ||
                            (r.DescriptionOfTasksInvolved != null && r.DescriptionOfTasksInvolved.Contains(searchString)) ||
                            (r.ScansTaken != null && r.ScansTaken.Contains(searchString)) ||
                            (r.ReceivingArchivist != null && r.ReceivingArchivist.Contains(searchString)) ||
                            (r.Notes != null && r.Notes.Contains(searchString)))
                .Include(r => r.CIdNavigation)
                .ToListAsync();
        }

        private async Task<List<Trunk>> SearchTrunks(string searchString)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ArchiveContext>();

            return await context.Trunks
                .Where(t => (t.BuildingNamePlanTitle != null && t.BuildingNamePlanTitle.Contains(searchString)) ||
                            (t.PlanYear != null && t.PlanYear.ToString().Contains(searchString)) ||
                            (t.ArchitectFirmAssociated != null && t.ArchitectFirmAssociated.Contains(searchString)) ||
                            (t.FolderName != null && t.FolderName.Contains(searchString)) ||
                            (t.Links != null && t.Links.Contains(searchString)) ||
                            (t.Notes != null && t.Notes.Contains(searchString)))
                .Include(t => t.CIdNavigation)
                .ToListAsync();
        }

        private CrossTable ApplyPagination(CrossTable model, int pageNumber, int pageSize)
        {
            if (pageSize == -1)
                return model;

            // Calculate total items to skip
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            int totalProcessed = 0;

            var paginatedModel = new CrossTable();

            // Process AudioFiles
            if (skip < model.AudioFiles.Count)
            {
                int toTake = Math.Min(take, model.AudioFiles.Count - skip);
                paginatedModel.AudioFiles = model.AudioFiles.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.AudioFiles.Count;
                paginatedModel.AudioFiles = new List<AudioFile>();
            }

            // Process Collections
            if (take > 0 && skip < model.Collections.Count)
            {
                int toTake = Math.Min(take, model.Collections.Count - skip);
                paginatedModel.Collections = model.Collections.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Collections.Count;
                paginatedModel.Collections = new List<Collection>();
            }
            // Process Delanceys
            if (take > 0 && skip < model.Delanceys.Count)
            {
                int toTake = Math.Min(take, model.Delanceys.Count - skip);
                paginatedModel.Delanceys = model.Delanceys.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Delanceys.Count;
                paginatedModel.Delanceys = new List<Delancey>();
            }
            // Process Maps
            if (take > 0 && skip < model.Maps.Count)
            {
                int toTake = Math.Min(take, model.Maps.Count - skip);
                paginatedModel.Maps = model.Maps.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Maps.Count;
                paginatedModel.Maps = new List<Map>();
            }
            // Process Oversizeds
            if (take > 0 && skip < model.Oversizeds.Count)
            {
                int toTake = Math.Min(take, model.Oversizeds.Count - skip);
                paginatedModel.Oversizeds = model.Oversizeds.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Oversizeds.Count;
                paginatedModel.Oversizeds = new List<Oversized>();
            }
            // Process Photos
            if (take > 0 && skip < model.Photos.Count)
            {
                int toTake = Math.Min(take, model.Photos.Count - skip);
                paginatedModel.Photos = model.Photos.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Photos.Count;
                paginatedModel.Photos = new List<Photo>();
            }
            // Process PoisonBooks
            if (take > 0 && skip < model.PoisonBooks.Count)
            {
                int toTake = Math.Min(take, model.PoisonBooks.Count - skip);
                paginatedModel.PoisonBooks = model.PoisonBooks.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.PoisonBooks.Count;
                paginatedModel.PoisonBooks = new List<PoisonBook>();
            }
            // Process ReportsPubs
            if (take > 0 && skip < model.ReportsPubs.Count)
            {
                int toTake = Math.Min(take, model.ReportsPubs.Count - skip);
                paginatedModel.ReportsPubs = model.ReportsPubs.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.ReportsPubs.Count;
                paginatedModel.ReportsPubs = new List<ReportsPub>();
            }
            // Process Researches
            if (take > 0 && skip < model.Researches.Count)
            {
                int toTake = Math.Min(take, model.Researches.Count - skip);
                paginatedModel.Researches = model.Researches.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Researches.Count;
                paginatedModel.Researches = new List<Research>();
            }
            // Process Trunks
            if (take > 0 && skip < model.Trunks.Count)
            {
                int toTake = Math.Min(take, model.Trunks.Count - skip);
                paginatedModel.Trunks = model.Trunks.Skip(skip).Take(toTake).ToList();
                totalProcessed += toTake;
                skip = 0;
                take -= toTake;
            }
            else
            {
                skip -= model.Trunks.Count;
                paginatedModel.Trunks = new List<Trunk>();
            }


            paginatedModel.TotalCount = model.TotalCount;

            return paginatedModel;
        }
    }
}
