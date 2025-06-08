using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
  public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
  {
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
      b.ToTable("OrderItems");
      b.HasKey(oi => oi.Id);
      b.Property(oi => oi.UnitPrice).HasColumnType("decimal(10,2)");

      b.HasOne(oi => oi.Order)
        .WithMany(o => o.Items)
        .HasForeignKey(oi => oi.OrderId)
        .OnDelete(DeleteBehavior.Cascade);

      b.HasOne(oi => oi.MenuItem)
        .WithMany(mi => mi.OrderItems)
        .HasForeignKey(oi => oi.MenuItemId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}