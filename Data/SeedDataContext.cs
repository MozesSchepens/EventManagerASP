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

            if (!context.Users.Any())
            {
                var adminUser = new ApplicationUser { UserName = "admin@event.com", Email = "admin@event.com", EmailConfirmed = true };
                var normalUser = new ApplicationUser { UserName = "user@event.com", Email = "user@event.com", EmailConfirmed = true };

                string adminPassword = "Admin@123";
                string userPassword = "User@123";

                var adminResult = await userManager.CreateAsync(adminUser, adminPassword);
                var userResult = await userManager.CreateAsync(normalUser, userPassword);

                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("✅ Admin-gebruiker aangemaakt en toegewezen aan rol 'Admin'.");
                }

                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");
                    logger.LogInformation("✅ Normale gebruiker aangemaakt en toegewezen aan rol 'User'.");
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
                var categories = await context.Categories.ToListAsync();
                var feestCategory = categories.FirstOrDefault(c => c.Name == "Feest")?.Id;
                var etenCategory = categories.FirstOrDefault(c => c.Name == "Eten")?.Id;
                var kerstCategory = categories.FirstOrDefault(c => c.Name == "Kerst")?.Id;
                var festivalCategory = categories.FirstOrDefault(c => c.Name == "Festival")?.Id;

                List<Event> events = new List<Event>
                {
                    new Event { Name = "BBQ Party", Description = "Heerlijke BBQ met vrienden", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = (int)etenCategory },
                    new Event { Name = "Kerstfeest", Description = "Gezellige kerstviering", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = (int)kerstCategory },
                    new Event { Name = "Zomerfestival", Description = "Groot festival met live muziek", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = (int)festivalCategory }
                };

                context.Events.AddRange(events);
                await context.SaveChangesAsync();
                logger.LogInformation("✅ Standaard evenementen succesvol toegevoegd.");
            }
        }
    }
}
