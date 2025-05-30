using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");
            b.HasKey(u => u.Id);
            b.Property(u => u.FullName).IsRequired().HasMaxLength(80);
            b.Property(u => u.Email).IsRequired().HasMaxLength(120);
            b.HasIndex(u => new { u.Email, u.RestaurantId }).IsUnique();
            b.Property(u => u.Role)
              .HasConversion<string>()
              .HasMaxLength(20);
        }
    }
}