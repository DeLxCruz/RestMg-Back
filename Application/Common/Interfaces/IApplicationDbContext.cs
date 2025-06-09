using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DatabaseFacade Database { get; }
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