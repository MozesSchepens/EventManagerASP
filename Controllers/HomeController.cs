using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EventManagerASP.Data;
using EventManagerASP.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            ViewBag.Events = new List<Event>();
        }
        else
        {
            ViewBag.Events = await _context.Events
                .Where(e => _context.Organisator.Any(o => o.EventId == e.Id && o.UserId == userId))
                .ToListAsync();
        }

        return View();
    }
}
