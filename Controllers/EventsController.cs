using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerASP.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(eventModel.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", "Ongeldige categorie geselecteerd.");
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(eventModel);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                eventModel.StartedById = user?.Id ?? string.Empty;

                _context.Events.Add(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(eventModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

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
        public async Task<IActionResult> Edit(int id, Event eventModel)
        {
            if (id != eventModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(eventModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventModel == null)
                return NotFound();

            return View(eventModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
                return NotFound();

            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
