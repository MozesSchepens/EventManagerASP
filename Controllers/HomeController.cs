using EventManagerASP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EventManagerASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Initialize ViewBag.Events with a non-null value
            ViewBag.Events = GetEvents() ?? new List<Event>(); // Replace GetEvents() with your actual method to fetch events
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

        // Example method to fetch events (replace with your actual implementation)
        private List<Event> GetEvents()
        {
            // Fetch events from your data source
            return new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" }
            };
        }
    }
}
