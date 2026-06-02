using Microsoft.EntityFrameworkCore;
using purchase_service.Models;

namespace purchase_service.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("purchase");

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.TouristId).IsRequired();
            entity.Property(c => c.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.HasIndex(c => c.TouristId).IsUnique();

            entity.HasMany(c => c.Items)
                .WithOne(i => i.ShoppingCart)
                .HasForeignKey(i => i.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.TourId).IsRequired();
            entity.Property(i => i.TourName).IsRequired();
            entity.Property(i => i.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.HasIndex(i => new { i.ShoppingCartId, i.TourId }).IsUnique();
        });

        modelBuilder.Entity<TourPurchaseToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.TouristId).IsRequired();
            entity.Property(t => t.TourId).IsRequired();
            entity.Property(t => t.Token).IsRequired();
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.HasIndex(t => new { t.TouristId, t.TourId }).IsUnique();
        });
    }
}
