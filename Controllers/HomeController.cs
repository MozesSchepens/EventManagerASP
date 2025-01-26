using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EventManagerASP.Data;
using EventManagerASP.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Localization;

public class ContactFormModel
{
    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Message { get; set; }
}

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactFormModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["SuccessMessage"] = "Bedankt voor je bericht! We nemen snel contact met je op.";
            return RedirectToAction("Contact");
        }

        return View(model);
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        IEnumerable<Event> events;

        if (string.IsNullOrEmpty(userId))
        {
            events = await _context.Events.Include(e => e.Category).ToListAsync();
        }
        else
        {
            events = await _context.Events
                .Include(e => e.Category)
                .Where(e => _context.Organisators.Any(o => o.EventId == e.Id && o.UserId == userId))
                .ToListAsync();
        }

        return View(events);
    }

    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }
}
