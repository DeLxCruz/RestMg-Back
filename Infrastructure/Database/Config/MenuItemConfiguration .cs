using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> b)
        {
            b.ToTable("MenuItems");
            b.HasKey(m => m.Id);
            b.Property(m => m.Name).IsRequired().HasMaxLength(120);
            b.Property(m => m.Price).HasColumnType("decimal(10,2)");
            b.HasOne(m => m.Category)
              .WithMany(c => c.Items)
              .HasForeignKey(m => m.CategoryId);
        }
    }
}