using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

public class MyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MyMiddleware> _logger;

    public MyMiddleware(RequestDelegate next, ILogger<MyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureFeature = context.Features.Get<IRequestCultureFeature>();
        var currentCulture = cultureFeature?.RequestCulture.Culture ?? new CultureInfo("en-US");

        _logger.LogInformation($"Actieve taal: {currentCulture.Name}");

        var cultureCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
        if (!string.IsNullOrEmpty(cultureCookie))
        {
            var culture = CookieRequestCultureProvider.ParseCookieValue(cultureCookie).Cultures[0].Value;
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

        await _next(context);
    }
}
