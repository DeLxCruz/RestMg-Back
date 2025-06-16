using Application.Common.Interfaces;
using Application.Common.Notifications;
using Application.Features.Kitchen.Notifications;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken ct)
        {
            var table = await db.Tables.FindAsync(command.TableId) ?? throw new KeyNotFoundException("Mesa no encontrada.");
            if (table.RestaurantId != command.RestaurantId) throw new UnauthorizedAccessException("La mesa no pertenece a este restaurante.");
            if (table.Status != TableStatus.Available) throw new InvalidOperationException("La mesa no está disponible para un nuevo pedido.");

            var menuItemIds = command.Items.Select(i => i.MenuItemId).ToList();
            var menuItemsFromDb = await db.MenuItems
                .Where(mi => mi.RestaurantId == command.RestaurantId && menuItemIds.Contains(mi.Id) && mi.IsAvailable)
                .ToDictionaryAsync(mi => mi.Id, mi => mi, ct);

            if (menuItemIds.Count != menuItemsFromDb.Count)
                throw new InvalidOperationException("Uno o más platos no están disponibles o no existen.");

            var orderItems = new List<OrderItem>();
            var notificationItems = new List<OrderItemNotificationDto>();
            decimal total = 0;

            foreach (var itemCommand in command.Items)
            {
                var menuItem = menuItemsFromDb[itemCommand.MenuItemId];
                orderItems.Add(new OrderItem { MenuItemId = menuItem.Id, Quantity = itemCommand.Quantity, UnitPrice = menuItem.Price });
                notificationItems.Add(new OrderItemNotificationDto(menuItem.Name, itemCommand.Quantity));
                total += menuItem.Price * itemCommand.Quantity;
            }

            table.Status = TableStatus.Occupied; // Cambiar estado de la mesa

            var newOrder = new Order
            {
                RestaurantId = command.RestaurantId,
                TableId = command.TableId,
                Total = total,
                Items = orderItems,
                Status = OrderStatus.AwaitingPayment,
                OrderCode = GenerateOrderCode()
            };

            await db.Orders.AddAsync(newOrder, ct);
            await db.SaveChangesAsync(ct);

            return new CreateOrderResult(newOrder.Id, newOrder.OrderCode);
        }

        private static string GenerateOrderCode() => new Random().Next(1000, 9999).ToString("D4");
    }
}