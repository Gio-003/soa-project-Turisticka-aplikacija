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

        // DbSets
        public DbSet<KeyPoints> KeyPoints { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourTag> TourTags { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tour");

            base.OnModelCreating(modelBuilder);

            // =========================
            // KEYPOINTS
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

                // RELATION: KeyPoints -> Tour
                entity.HasOne(kp => kp.Tour)
                      .WithMany(t => t.KeyPoints)
                      .HasForeignKey(kp => kp.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
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
            // REVIEW
            // =========================
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.TourId).IsRequired();
                entity.Property(r => r.ReviewerUserId).IsRequired();
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Comment).IsRequired();
                entity.Property(r => r.VisitDate).IsRequired();
                entity.Property(r => r.ReviewDate).IsRequired();
                entity.Property(r => r.Images).HasDefaultValue("[]");
                entity.Property(r => r.ReviewerName);
                entity.Property(r => r.ReviewerProfilePicture);

                // RELATION: Review -> Tour
                entity.HasOne(r => r.Tour)
                      .WithMany(t => t.Reviews)
                      .HasForeignKey(r => r.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}