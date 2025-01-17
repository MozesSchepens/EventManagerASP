using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagerADV.Models;
using EventManagerADV.ViewModels;

namespace EventManagerADV.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EventManagerADV.Models.Event> Events { get; set; } = default!;
        public DbSet<EventManagerADV.Models.Category> Categories { get; set; } = default!;
        public DbSet<EventManagerADV.Models.Organisator> Organisator { get; set; } = default!;
    }
}
