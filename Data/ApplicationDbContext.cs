using EventManagerASP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EventManagerASP.Models;


namespace EventManagerASP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Organisator> Organisator { get; set; } = default!;
    }
}
