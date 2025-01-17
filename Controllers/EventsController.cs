using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventManagerADV.Data;
using EventManagerADV.Models;
using EventManagerADV.Services;

namespace EventManagerADV.Controllers
{
    [Authorize(Roles = "User")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMyUser _userService;

        public EventsController(ApplicationDbContext context, IMyUser userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Where(e => e.Deleted > DateTime.Now)
                .Include(e => e.Category)
                .OrderBy(e => e.Name)
                .ToListAsync();

            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventModel == null) return NotFound();

            return View(eventModel);
        }

        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories.Where(c => c.Deleted > DateTime.Now), "Id", "Name");
            return View(new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate,EstimatedBudget,CategoryId,Deleted")] Event eventModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                eventModel.StartedById = user?.Id ?? "?";

                _context.Add(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(_context.Categories.Where(c => c.Deleted > DateTime.Now), "Id", "Name", eventModel.CategoryId);
            return View(eventModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null) return NotFound();

            ViewData["Categories"] = new SelectList(_context.Categories.Where(c => c.Deleted > DateTime.Now), "Id", "Name", eventModel.CategoryId);
            return View(eventModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventModel == null) return NotFound();

            return View(eventModel);
        }
    }
}
