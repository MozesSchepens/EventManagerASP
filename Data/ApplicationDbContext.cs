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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Organisator>()
                .HasOne(o => o.OrganisatorUser)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Organisator>()
                .HasOne(o => o.Event)
                .WithMany()
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Organisator>()
                .HasOne(o => o.DoneBy)
                .WithMany()
                .HasForeignKey(o => o.DoneById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Organisator>()
                .HasOne(o => o.OrgUser)
                .WithMany()
                .HasForeignKey(o => o.OrgId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }

}