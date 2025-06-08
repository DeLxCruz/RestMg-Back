using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("Categories");
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).IsRequired().HasMaxLength(60);
            b.HasOne(c => c.Restaurant)
              .WithMany(r => r.Categories)
              .HasForeignKey(c => c.RestaurantId);
        }
    }
}