using Microsoft.EntityFrameworkCore;
using tour_service.Models;

namespace tour_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // EXISTING
        public DbSet<KeyPoints> KeyPoints { get; set; }

        // NEW
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourTag> TourTags { get; set; }
        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tour");

            base.OnModelCreating(modelBuilder);

            // =========================
            // KEYPOINTS (existing)
            // =========================
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

            // =========================
            // TOUR
            // =========================
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name).IsRequired();
                entity.Property(t => t.Description).IsRequired();
                entity.Property(t => t.Difficulty).IsRequired();

                entity.Property(t => t.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(t => t.Status)
                      .IsRequired();

                // RELATION: Tour -> KeyPoints (ako KeyPoints pripada turi)
                entity.HasMany<KeyPoints>()
                      .WithOne()
                      .HasForeignKey(kp => kp.TourId)
                      .OnDelete(DeleteBehavior.Cascade);

                // RELATION: Tour -> Tags
                entity.HasMany(t => t.Tags)
                      .WithOne(tt => tt.Tour)
                      .HasForeignKey(tt => tt.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // TOUR TAG
            // =========================
            modelBuilder.Entity<TourTag>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name).IsRequired();

                entity.Property(t => t.TourId).IsRequired();
            });

            // =========================
            // USER
            // =========================
            /*modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username).IsRequired();

                entity.HasMany(u => u.Tours)
                      .WithOne(t => t.Author)
                      .HasForeignKey(t => t.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });*/
        }
    }
}