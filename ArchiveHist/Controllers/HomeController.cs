using ArchiveHist.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ArchiveHist.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArchiveContext _context;

        public HomeController(ILogger<HomeController> logger, ArchiveContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            string username = User.Identity?.Name;
            if (!string.IsNullOrEmpty(username) && username.Contains("\\"))
            {
                username = username.Split('\\')[1]; // username without domain
            }

            // Get user from database
            var user = _context.Users.FirstOrDefault(u => u.Name == username);

            ViewBag.CurrentUser = user;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous] // Allows access without authorization
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
