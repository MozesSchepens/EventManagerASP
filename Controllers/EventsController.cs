using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerASP.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext context, ILogger<EventsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _context.Events.Include(e => e.Category).ToListAsync();
                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading events: {ex.Message}");
                TempData["ErrorMessage"] = "Evenementen kunnen momenteel niet geladen worden.";
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Events.Add(eventModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating event: {ex.Message}");
                    TempData["ErrorMessage"] = "Fout bij het aanmaken van evenement.";
                    return RedirectToAction("Error", "Home");
                }
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(eventModel);
        }
    }
}
