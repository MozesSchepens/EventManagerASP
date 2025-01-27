using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Threading.Tasks;
using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace EventManagerASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStringLocalizer<SharedResource> localizer, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van evenementenlijst in HomeController.Index");
                TempData["ErrorMessage"] = _localizer["Er kon geen lijst met evenementen worden geladen."];
                return RedirectToAction("Error");
            }
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

        public IActionResult Error(int? statusCode = null)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var originalPath = feature?.OriginalPath;  // Geeft de originele URL van de fout terug

            // Controleer of het een 404 is en toon een specifieke boodschap
            string message = statusCode switch
            {
                404 => "De pagina die u zoekt, bestaat niet.",
                403 => "U hebt geen toegang tot deze pagina.",
                _ => "Er is een fout opgetreden. Probeer het later opnieuw."
            };

            var errorViewModel = new ErrorViewModel
            {
                StatusCode = statusCode ?? 500,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                OriginalPath = originalPath ?? "Onbekend",
                Message = message
            };

            return View(errorViewModel);
        }

        public IActionResult TestError()
        {
            throw new Exception("Dit is een testfout om de errorpagina te testen.");
        }
    }
}
