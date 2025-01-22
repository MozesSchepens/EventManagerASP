using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventManagerASP.Data;
using EventManagerASP.Models;

namespace EventManagerASP.Controllers
{
    [Authorize]
    public class OrganisatorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganisatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organisators
        public async Task<IActionResult> Index(int eventId)
        {
            var organisators = await _context.Organisator
                                             .Where(o => o.EventId == eventId && (o.Deleted == null || o.Deleted > DateTime.UtcNow))
                                             .Include(o => o.OrganisatorUser)
                                             .Include(o => o.Event)
                                             .ToListAsync();

            ViewData["EventId"] = eventId;
            return View(organisators);
        }

        // GET: Organisators/Create
        public IActionResult Create(int eventId)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
                return Unauthorized();

            var organisator = new Organisator { DoneById = currentUser.Id, EventId = eventId };

            ViewData["OrgId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", eventId);
            return View(organisator);
        }

        // POST: Organisators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,OrgId,BoDate,DoneById,Deleted")] Organisator organisator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(organisator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { eventId = organisator.EventId });
            }

            ViewData["OrgId"] = new SelectList(_context.Users, "Id", "UserName", organisator.OrgId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", organisator.EventId);
            return View(organisator);
        }

        // POST: Organisators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organisator = await _context.Organisator.FindAsync(id);
            if (organisator != null)
            {
                organisator.Deleted = DateTime.UtcNow; // Soft delete
                _context.Update(organisator);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { eventId = organisator?.EventId });
        }

        private bool OrganisatorExists(int id)
        {
            return _context.Organisator.Any(o => o.Id == id);
        }
    }
}
