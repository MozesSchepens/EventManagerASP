using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Threading.Tasks;
using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using EventManagerASP;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var events = userId == null
            ? await _context.Events.Include(e => e.Category).ToListAsync()
            : await _context.Events
                .Where(e => _context.Organisators.Any(o => o.EventId == e.Id && o.UserId == userId))
                .Include(e => e.Category)
                .ToListAsync();

        ViewData["Title"] = _localizer["Home"];
        return View(events);
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = _localizer["Contact"];
        return View();
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
