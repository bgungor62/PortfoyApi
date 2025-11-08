using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfoyApi.Models;

namespace PortfoyApi.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Portfoy> Portfoys => Set<Portfoy>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfoy>()
                .HasIndex(b => b.Slug)
                .IsUnique();

            builder.Entity<Portfoy>()
                .HasIndex(b => b.UserId);
        }
    }
}
