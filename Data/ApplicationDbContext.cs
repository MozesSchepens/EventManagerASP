using EventManagerASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Organisator> Organisators { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
