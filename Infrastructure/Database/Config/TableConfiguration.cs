using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
    public class TableConfiguration : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> b)
        {
            b.ToTable("Tables");
            b.HasKey(t => t.Id);
            b.Property(t => t.Code).IsRequired().HasMaxLength(10);
            b
              .HasIndex(t => new { t.RestaurantId, t.Code })
              .IsUnique();
        }
    }
}