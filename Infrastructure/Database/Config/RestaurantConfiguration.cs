using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> b)
        {
            b.ToTable("Restaurants");
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).IsRequired().HasMaxLength(120);
            b.Property(r => r.BrandingColor).HasMaxLength(12);
            b.Property(r => r.LogoUrl).HasMaxLength(300);
        }
    }
}