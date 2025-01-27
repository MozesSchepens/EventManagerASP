using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerASP.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<EventsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.Include(e => e.Category).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Event eventModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Ongeldige invoer." });
            }

            var category = await _context.Categories.FindAsync(eventModel.CategoryId);
            if (category == null)
            {
                return Json(new { success = false, message = "Ongeldige categorie geselecteerd." });
            }

            var user = await _userManager.GetUserAsync(User);
            eventModel.StartedById = user?.Id ?? string.Empty;

            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Evenement aangemaakt!", Event = eventModel });
        }

        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await _context.Events.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] Event eventModel)
        {
            if (id != eventModel.Id)
                return Json(new { success = false, message = "Ongeldig evenement-ID." });

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Ongeldige invoer." });
            }

            try
            {
                _context.Update(eventModel);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Evenement bijgewerkt!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Events.Any(e => e.Id == id))
                {
                    return Json(new { success = false, message = "Evenement niet gevonden." });
                }
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
            if (eventModel == null)
                return NotFound();

            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
                return Json(new { success = false, message = "Evenement niet gevonden." });

            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Evenement verwijderd!" });
        }
    }
}
