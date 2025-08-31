using Domain.Enums;

namespace Domain.Models
{
    public class Order
    {
        public Order()
        {
            var colombianZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombianZone);
            Items = new List<OrderItem>();
        }

        public Guid Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
        public Guid TableId { get; set; }
        public Table Table { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Total { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}