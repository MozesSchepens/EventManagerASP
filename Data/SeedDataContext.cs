using EventManagerASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerASP.Data
{
    public class SeedDataContext
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SeedDataContext> logger)
        {
            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation($"✅ Rol '{role}' aangemaakt.");
                }
            }

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Feest" },
                    new Category { Name = "Eten" },
                    new Category { Name = "Kerst" },
                    new Category { Name = "Festival" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
                logger.LogInformation("✅ Categorieën succesvol toegevoegd.");
            }

            if (!context.Events.Any())
            {
                var feestCategory = await context.Categories.Where(c => c.Name == "Feest").Select(c => c.Id).FirstOrDefaultAsync();
                var etenCategory = await context.Categories.Where(c => c.Name == "Eten").Select(c => c.Id).FirstOrDefaultAsync();
                var kerstCategory = await context.Categories.Where(c => c.Name == "Kerst").Select(c => c.Id).FirstOrDefaultAsync();
                var festivalCategory = await context.Categories.Where(c => c.Name == "Festival").Select(c => c.Id).FirstOrDefaultAsync();

                List<Event> events = new List<Event>
                {
                    new Event { Name = "BBQ Party", Description = "Heerlijke BBQ met vrienden", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = etenCategory },
                    new Event { Name = "Kerstfeest", Description = "Gezellige kerstviering", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = kerstCategory },
                    new Event { Name = "Zomerfestival", Description = "Groot festival met live muziek", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = festivalCategory }
                };

                context.Events.AddRange(events);
                await context.SaveChangesAsync();
                logger.LogInformation("✅ Standaard evenementen succesvol toegevoegd.");
            }
        }
    }
}
