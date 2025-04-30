using HostelMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HostelMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured hostels with management type information
            var institutionHostels = await _context.Hostels
                .Where(h => h.IsActive && h.ManagementType == ManagementType.InstitutionManaged)
                .Take(3)
                .ToListAsync();
                
            var privateHostels = await _context.Hostels
                .Where(h => h.IsActive && h.ManagementType == ManagementType.PrivatelyManaged)
                .Take(3)
                .ToListAsync();

            ViewBag.InstitutionHostels = institutionHostels;
            ViewBag.PrivateHostels = privateHostels;

            // Return all hostels to maintain compatibility with existing view
            return View(institutionHostels.Concat(privateHostels).ToList());
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
    }
}