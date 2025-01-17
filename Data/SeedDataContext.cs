using EventManagerADV.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagerADV.Data
{
    public class SeedDataContext
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            await context.Database.MigrateAsync();

            string[] roleNames = { "User", "UserAdmin", "SystemAdmin" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // ✅ 2. Gebruikers aanmaken als ze nog niet bestaan
            if (!await userManager.Users.AnyAsync())
            {
                var testUser = new Users { UserName = "TestUser", FirstName = "Test", LastName = "User", Email = "test@example.com", EmailConfirmed = true };
                var adminUser = new Users { UserName = "SystemAdmin", FirstName = "System", LastName = "Admin", Email = "admin@example.com", EmailConfirmed = true };

                await userManager.CreateAsync(testUser, "Xxx!12345");
                await userManager.CreateAsync(adminUser, "Xxx!12345");

                await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                await userManager.AddToRoleAsync(testUser, "UserAdmin");
            }

            await context.SaveChangesAsync();

            if (!context.Events.Any())
            {
                var defaultUser = await userManager.FindByNameAsync("SystemAdmin");

                List<Event> events = new List<Event>
                {
                    new Event { Name = "Drank of je Leven", Description = "Gezellige avond met drank", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 1, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Spelletjes avond", Description = "Een avond vol bordspellen", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 1, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Eureka BBQ", Description = "Heerlijke barbecue met vrienden", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 2, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Kerstfeest Fantomas", Description = "Kerstfeest met cadeaus en eten", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 3, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Stoempkermis", Description = "Traditionele Belgische kermis", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 4, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Koerrock", Description = "Festival met live muziek", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 5, StartedById = defaultUser?.Id ?? "?" },
                    new Event { Name = "Verjaardag Casi", Description = "Verjaardagsfeest voor Casi", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), CategoryId = 6, StartedById = defaultUser?.Id ?? "?" }
                };

                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }
        }
    }
}
