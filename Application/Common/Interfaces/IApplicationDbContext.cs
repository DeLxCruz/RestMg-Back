using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Restaurant> Restaurants { get; }
        DbSet<Table> Tables { get; }
        DbSet<Category> Categories { get; }
        DbSet<MenuItem> MenuItems { get; }
        DbSet<User> Users { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}