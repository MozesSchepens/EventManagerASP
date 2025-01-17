using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventManagerADV.Data;
using EventManagerADV.Models;

namespace EventManagerADV.Controllers
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
                                             .Where(o => o.EventId == eventId && o.Deleted > DateTime.Now)
                                             .Include(o => o.OrganisatorUser)
                                             .Include(o => o.Event)
                                             .ToListAsync();

            ViewData["EventId"] = eventId;
            return View(organisators);
        }

        // GET: Organisators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var organisatorModel = await _context.Organisator
                                                 .Include(o => o.OrganisatorUser)
                                                 .Include(o => o.Event)
                                                 .FirstOrDefaultAsync(o => o.Id == id);

            if (organisatorModel == null)
                return NotFound();

            return View(organisatorModel);
        }

        // GET: Organisators/Create
        public IActionResult Create(int eventId)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
                return Unauthorized();

            var organisator = new Organisator { DoneById = currentUser.Id, EventId = eventId };

            ViewData["OrgId"] = new SelectList(_context.Users.Where(u => u.UserName != "?"), "Id", "UserName");
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Description", eventId);
            return View(organisator);
        }

        // POST: Organisators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,OrgId,BoDate,DoneById,Remark,Deleted")] Organisator organisator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(organisator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { eventId = organisator.EventId });
            }

            ViewData["OrgId"] = new SelectList(_context.Users.Where(u => u.UserName != "?"), "Id", "UserName", organisator.OrgId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Description", organisator.EventId);
            return View(organisator);
        }

        // GET: Organisators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var organisatorModel = await _context.Organisator.FindAsync(id);
            if (organisatorModel == null)
                return NotFound();

            ViewData["OrgId"] = new SelectList(_context.Users, "Id", "UserName", organisatorModel.OrgId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Description", organisatorModel.EventId);
            return View(organisatorModel);
        }

        // POST: Organisators/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,OrgId,BoDate,DoneById,Remark,Deleted")] Organisator organisator)
        {
            if (id != organisator.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organisator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganisatorExists(organisator.Id))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index), new { eventId = organisator.EventId });
            }

            ViewData["OrgId"] = new SelectList(_context.Users, "Id", "UserName", organisator.OrgId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Description", organisator.EventId);
            return View(organisator);
        }

        // GET: Organisators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var organisatorModel = await _context.Organisator
                                                 .Include(o => o.OrganisatorUser)
                                                 .Include(o => o.Event)
                                                 .FirstOrDefaultAsync(o => o.Id == id);

            if (organisatorModel == null)
                return NotFound();

            return View(organisatorModel);
        }

        // POST: Organisators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organisatorModel = await _context.Organisator.FindAsync(id);
            if (organisatorModel != null)
            {
                _context.Organisator.Remove(organisatorModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { eventId = organisatorModel?.EventId });
        }

        private bool OrganisatorExists(int id)
        {
            return _context.Organisator.Any(o => o.Id == id);
        }
    }
}
