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
        public DbSet<TourDuration> TourDurations { get; set; } // NOVO
        public DbSet<TourExecution> TourExecutions { get; set; }
        public DbSet<CompletedKeyPoint> CompletedKeyPoints { get; set; }

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
                entity.Property(t=>t.LengthInKm).IsRequired();

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
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Comment).IsRequired();
                entity.Property(r => r.TouristId).IsRequired();
                entity.Property(r => r.TouristUsername).IsRequired();
                entity.Property(r => r.VisitDate).IsRequired();
                entity.Property(r => r.CreatedAt).IsRequired();

                // Store Images as JSON array
                entity.Property(r => r.Images)
                      .HasConversion(
                          v => string.Join(",", v),
                          v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split(new char[] { ',' }).ToList()
                      );

                // RELATION: Review -> Tour
                entity.HasOne(r => r.Tour)
                      .WithMany()
                      .HasForeignKey(r => r.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // TOUR DURATION (NOVO)
            // =========================
            modelBuilder.Entity<TourDuration>(entity =>
            {
                entity.HasKey(td => td.Id);

                entity.Property(td => td.TransportType)
                      .IsRequired();

                entity.Property(td => td.DurationInMinutes)
                      .IsRequired();

                entity.Property(td => td.TourId)
                      .IsRequired();

                // RELATION: TourDuration -> Tour
                entity.HasOne(td => td.Tour)
                      .WithMany(t => t.Durations)
                      .HasForeignKey(td => td.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // TOUR EXECUTION
            // =========================
            modelBuilder.Entity<TourExecution>(entity =>
            {
                entity.HasKey(te => te.Id);
                entity.Property(te => te.TouristId).IsRequired();
                entity.Property(te => te.Status).IsRequired();
                entity.Property(te => te.StartedAt).IsRequired();
                entity.Property(te => te.LastActivity).IsRequired();
                entity.Property(te => te.StartLatitude).IsRequired();
                entity.Property(te => te.StartLongitude).IsRequired();

                entity.HasOne(te => te.Tour)
                      .WithMany()
                      .HasForeignKey(te => te.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // COMPLETED KEY POINT
            // =========================
            modelBuilder.Entity<CompletedKeyPoint>(entity =>
            {
                entity.HasKey(ckp => ckp.Id);
                entity.Property(ckp => ckp.KeyPointId).IsRequired();
                entity.Property(ckp => ckp.CompletedAt).IsRequired();

                entity.HasOne(ckp => ckp.TourExecution)
                      .WithMany(te => te.CompletedKeyPoints)
                      .HasForeignKey(ckp => ckp.TourExecutionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}