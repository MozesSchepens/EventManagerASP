using EventManagerASP.Services;
using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Infrastructure.Internal;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// **DATABASE: GEBRUIK SQLITE**
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=eventmanager.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// **IDENTITY CONFIG**
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// **EMAIL VERZENDING**
builder.Services.Configure<MailKitOptions>(builder.Configuration.GetSection("ExternalProviders:MailKit:SMTP"));
builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

// **MEERTALIGE ONDERSTEUNING**
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        var type = typeof(EventManagerASP.SharedResource);
        var factory = builder.Services.BuildServiceProvider().GetRequiredService<IStringLocalizerFactory>();
        var localizer = factory.Create(type);
        options.DataAnnotationLocalizerProvider = (t, f) => localizer;
    });

// **TAALCONFIGURATIE**
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("fr-FR"),
    new CultureInfo("nl-NL")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventManagerASP", Version = "v1" });
});

builder.Services.AddTransient<IMyUser, MyUser>();

var app = builder.Build();
Globals.App = app;

// **FOUTEN AFVANGEN**
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
}
else
{
    app.UseDeveloperExceptionPage();
}

// **TAALOPTIES LADEN**
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

// **DATABASE MIGRATIE + SEEDING**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<SeedDataContext>>();

    try
    {
        context.Database.Migrate();
        SeedDataContext.Initialize(context, userManager, roleManager, logger).Wait();
        logger.LogInformation("✅ Database migratie voltooid en seeding succesvol.");
    }
    catch (Exception ex)
    {
        logger.LogError($"❌ Database seeding mislukt: {ex.Message}\n{ex.StackTrace}");
    }
}

// **MIDDLEWARE INLADEN**
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// **FOUTEN AFVANGEN IN REQUESTS**
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError($"❌ Onverwachte fout: {ex.Message}\n{ex.StackTrace}");
        context.Response.Redirect("/Home/Error?statusCode=500");
    }
});

// **ROUTES**
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
