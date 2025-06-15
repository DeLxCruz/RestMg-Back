using Application.Common.Interfaces;
using Application.Common.Notifications;
using Application.Features.Kitchen.Notifications;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(IApplicationDbContext db, IPublisher publisher)
        : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken ct)
        {
            // Validar mesa y restaurante
            var table = await db.Tables
                .FirstOrDefaultAsync(t => t.Id == command.TableId && t.RestaurantId == command.RestaurantId, ct)
                ?? throw new KeyNotFoundException("Mesa no encontrada.");

            // Validar MenuItems y construir los OrderItems
            var menuItemIds = command.Items.Select(i => i.MenuItemId).ToList();
            var menuItems = await db.MenuItems
                .Where(mi => mi.RestaurantId == command.RestaurantId && menuItemIds.Contains(mi.Id) && mi.IsAvailable)
                .ToDictionaryAsync(mi => mi.Id, mi => mi, ct);

            if (menuItemIds.Count != menuItems.Count)
                throw new InvalidOperationException("Uno o más platos no están disponibles o no existen.");

            decimal total = 0;
            var orderItems = new List<OrderItem>();
            var itemDtosForNotification = new List<OrderItemNotificationDto>();

            foreach (var itemCommand in command.Items)
            {
                var menuItem = menuItems[itemCommand.MenuItemId];
                var orderItem = new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Quantity = itemCommand.Quantity,
                    UnitPrice = menuItem.Price // Guardar el precio en el momento del pedido
                };
                total += orderItem.UnitPrice * orderItem.Quantity;
                orderItems.Add(orderItem);

                // Datos para la notificación
                itemDtosForNotification.Add(new OrderItemNotificationDto(menuItem.Name, itemCommand.Quantity));
            }

            // Crear el Pedido (Order)
            var newOrder = new Order
            {
                RestaurantId = command.RestaurantId,
                TableId = command.TableId,
                Total = total,
                Items = orderItems,
                OrderCode = GenerateOrderCode()
            };

            await db.Orders.AddAsync(newOrder, ct);
            await db.SaveChangesAsync(ct);

            // Publicar Notificaciones
            await PublishNotifications(publisher, newOrder, table, itemDtosForNotification, ct);

            return new CreateOrderResult(newOrder.Id, newOrder.OrderCode);
        }

        private static string GenerateOrderCode() => new Random().Next(1000, 9999).ToString("D4");

        private static async Task PublishNotifications(IPublisher publisher, Order order, Table table, List<OrderItemNotificationDto> itemDtos, CancellationToken ct)
        {
            // Notificación a la cocina sobre el nuevo pedido
            var orderNotification = new NewOrderReceivedNotification(
                order.RestaurantId,
                order.Id,
                table.Code,
                itemDtos,
                order.CreatedAt);
            await publisher.Publish(orderNotification, ct);

            // Notificación sobre el estado de la mesa
            var tableNotification = new TableStateChangedNotification(table.RestaurantId, table.Id, TableStatus.Occupied.ToString());
            await publisher.Publish(tableNotification, ct);
        }
    }
}