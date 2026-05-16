using Microsoft.EntityFrameworkCore;
using tour_service.Models;

namespace tour_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }
        public DbSet<KeyPoints> KeyPoints { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tour");

            modelBuilder.Entity<KeyPoints>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired();
                    entity.Property(e => e.Description).IsRequired();
                    entity.Property(e => e.ImageUrl).IsRequired();
                    entity.Property(e => e.TourId).IsRequired();
                    entity.Property(e => e.Longitude).IsRequired();
                    entity.Property(e => e.Latitude).IsRequired();
                });

        }
    }
}
