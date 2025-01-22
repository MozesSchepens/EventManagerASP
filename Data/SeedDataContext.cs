using EventManagerASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerASP.Data
{
    public class SeedDataContext
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SeedDataContext> logger)
        {
            context.Database.EnsureCreated();
            await context.Database.MigrateAsync();

            // ✅ Zorg ervoor dat de rollen bestaan voordat gebruikers worden aangemaakt
            string[] roleNames = { "User", "UserAdmin", "SystemAdmin" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation($"Creating role: {roleName}");
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (!await userManager.Users.AnyAsync(u => u.Email == "admin@example.com"))
            {
                logger.LogInformation("Seeding admin user...");

                var adminUser = new ApplicationUser
                {
                    UserName = "SystemAdmin",
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin!12345");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        logger.LogError($"Error creating admin user: {error.Description}");
                    }
                }
            }

            if (!context.Events.Any())
            {
                logger.LogInformation("Seeding events...");

                var categories = await context.Categories.ToListAsync();
                var feestCategory = categories.FirstOrDefault(c => c.Name == "Feest")?.Id ?? 1;
                var etenCategory = categories.FirstOrDefault(c => c.Name == "Eten")?.Id ?? 2;
                var kerstCategory = categories.FirstOrDefault(c => c.Name == "Kerst")?.Id ?? 3;
                var festivalCategory = categories.FirstOrDefault(c => c.Name == "Festival")?.Id ?? 4;

                var defaultUser = await userManager.FindByEmailAsync("admin@example.com");

                List<Event> events = new List<Event>
                {
                    new Event { Name = "BBQ Party", Description = "Heerlijke BBQ met vrienden", CategoryId = etenCategory, StartedById = defaultUser?.Id ?? string.Empty },
                    new Event { Name = "Kerstfeest", Description = "Gezellige kerstviering", CategoryId = kerstCategory, StartedById = defaultUser?.Id ?? string.Empty },
                    new Event { Name = "Zomerfestival", Description = "Groot festival met live muziek", CategoryId = festivalCategory, StartedById = defaultUser?.Id ?? string.Empty }
                };

                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }

            logger.LogInformation("Seeding completed.");
        }
    }
}
