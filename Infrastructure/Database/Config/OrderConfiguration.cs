using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
  public class OrderConfiguration : IEntityTypeConfiguration<Order>
  {
    public void Configure(EntityTypeBuilder<Order> b)
    {
      b.ToTable("Orders");
      b.HasKey(o => o.Id);

      b.Property(o => o.Total)
        .HasColumnType("decimal(10,2)");

      b.Property(o => o.Status)
        .HasConversion<string>()
        .HasMaxLength(20);

      b.HasOne(o => o.Table)
        .WithMany(t => t.Orders)
        .HasForeignKey(o => o.TableId)
        .OnDelete(DeleteBehavior.Cascade);

      b.HasOne(o => o.Restaurant)
        .WithMany(r => r.Orders)
        .HasForeignKey(o => o.RestaurantId)
        .OnDelete(DeleteBehavior.Restrict);

      b.Property(o => o.OrderCode).IsRequired().HasMaxLength(10);

      b.Property(o => o.CompletedAt).IsRequired(false);

      b.HasIndex(o => new { o.RestaurantId, o.OrderCode }).IsUnique();
    }
  }
}