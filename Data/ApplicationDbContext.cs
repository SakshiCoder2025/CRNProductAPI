using Microsoft.EntityFrameworkCore;
using CRNProductAPI.Models;
using CRNProductAPI.Authentication;

namespace CRNProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ModifiedBy).HasMaxLength(100);
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Quantity).IsRequired();

                entity.HasOne(i => i.Product)
                      .WithMany(p => p.Items)
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}